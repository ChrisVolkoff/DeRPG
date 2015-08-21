using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;
using SkinnedModel;

namespace AtelierXNA
{
    public class BoundingBoxBuffers
    {
        public VertexBuffer Vertices;
        public int VertexCount;
        public IndexBuffer Indices;
        public int PrimitiveCount;
    }
    public class ObjetDeBasePhysique : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected bool Animated { get; set; }

        protected RPG Jeu { get; private set; }
        protected ScèneDeJeu ScèneJeu { get; private set; }
        private string NomModèle { get; set; }
        public float Échelle { get; protected set; }
        public Vector3 Rotation { get; protected set; }
        public Vector3 RotationOffset { get; protected set; }
        public Vector3 Position { get; protected set; }
        private float TempsÉcouléDepuisMAJ { get; set; }
        protected Model Modèle { get; private set; }
        protected Matrix[] TransformationsModèle { get; private set; }
        protected Matrix Monde { get; set; }
        float ÉchelleBox { get; set; }

        protected AnimationPlayer AnimPlayer { get; set; }

        public List<BoundingBox> BoxList { get; private set; }
        // BoundingBox drawing stuff
        public List<BoundingBoxBuffers> BoxDrawList { get; private set; }
        public BasicEffect lineEffect { get; protected set; }
        public bool DrawBoxes { get; set; }

        public ObjetDeBasePhysique(RPG jeu, ScèneDeJeu scèneJeu, String nomModèle, float échelleInitiale, float échelleBox, Vector3 rotationInitiale, Vector3 rotationOffset, Vector3 positionInitiale)
            : base(jeu)
        {
            Jeu = jeu;
            ScèneJeu = scèneJeu;
            NomModèle = nomModèle;
            Position = positionInitiale;
            Échelle = échelleInitiale;
            ÉchelleBox = échelleBox;
            Rotation = new Vector3(rotationInitiale.X, rotationInitiale.Y, rotationInitiale.Z);
            RotationOffset = rotationOffset;
        }

        public override void Initialize()
        {
            DrawBoxes = false;
            TempsÉcouléDepuisMAJ = 0;
            Modèle = ScèneJeu.GestionnaireDeModèles.Find(NomModèle);
            TransformationsModèle = new Matrix[Modèle.Bones.Count];
            Modèle.CopyAbsoluteBoneTransformsTo(TransformationsModèle);

            UpdateWorld();

            BoxList = new List<BoundingBox>();
            BoxDrawList = new List<BoundingBoxBuffers>();

            CreateBoxList();

            base.Initialize();
        }
        protected override void LoadContent()
        {
            SkinningData skinningData = Modèle.Tag as SkinningData;

            if (skinningData != null)
            {
                Animated = true;
                AnimPlayer = new AnimationPlayer(skinningData);
                AnimPlayer.StartClip(skinningData.AnimationClips["Take 001"]);
            }
            else
            {
                Animated = false;
            }

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {

            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TimeSpan accelTime = GérerTimeScale(gameTime);
            TempsÉcouléDepuisMAJ += tempsÉcoulé;
            if (Animated)
            {
                AnimPlayer.Update(accelTime, true, Matrix.Identity);
            }
            if (TempsÉcouléDepuisMAJ > Jeu.INTERVALLE_MAJ)
            {
                UpdateWorld();
                CreateBoxList();
                TempsÉcouléDepuisMAJ = 0;
            }
            GérerClavier();
            base.Update(gameTime);
        }

        protected virtual TimeSpan GérerTimeScale(GameTime gameTime)
        {
            return new TimeSpan((int)(gameTime.ElapsedGameTime.Ticks));
        }

        public override void Draw(GameTime gameTime)
        {
            if (Animated)
            {
                Matrix[] bones = AnimPlayer.GetSkinTransforms();

                // Compute camera matrices.
                Matrix view = ScèneJeu.CaméraJeu.Vue;

                Matrix projection = ScèneJeu.CaméraJeu.Projection;

                // Render the skinned mesh.
                foreach (ModelMesh mesh in Modèle.Meshes)
                {
                    Matrix mondeLocal = TransformationsModèle[mesh.ParentBone.Index] * GetMonde();
                    foreach (SkinnedEffect effect in mesh.Effects)
                    {
                        effect.SetBoneTransforms(bones);

                        effect.View = view;
                        effect.Projection = projection;
                        effect.World = mondeLocal;

                        effect.EnableDefaultLighting();
                    }
                    mesh.Draw();
                }
            }
            else
            {
                foreach (ModelMesh maille in Modèle.Meshes)
                {
                    Vector3[] oldColor = new Vector3[maille.MeshParts.Count];
                    int i = 0;
                    Matrix mondeLocal = TransformationsModèle[maille.ParentBone.Index] * GetMonde();
                    foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                    {
                        BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                        oldColor[i] = effet.EmissiveColor;
                        effet.EnableDefaultLighting();
                        GérerSurlignage(ref effet);
                        GérérCouleurCharm(ref effet);
                        effet.Projection = ScèneJeu.CaméraJeu.Projection;
                        effet.View = ScèneJeu.CaméraJeu.Vue;
                        effet.World = mondeLocal;
                        i++;
                    }
                    maille.Draw();
                    i = 0;
                    foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                    {
                        BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                        effet.EmissiveColor = oldColor[i];
                        i++;
                    }
                }
            }

            //Draw boxes
            if (DrawBoxes)
            {
                //Box effect
                if (lineEffect == null) // Comme ça on a qu'à le créer une seule fois (et on ne peut le créer dans le Initialize())
                {
                    lineEffect = new BasicEffect(ScèneJeu.GraphicsDevice);
                    lineEffect.LightingEnabled = false;
                    lineEffect.TextureEnabled = false;
                    lineEffect.VertexColorEnabled = true;
                }

                foreach (BoundingBoxBuffers bb in BoxDrawList)
                {
                    DrawBoundingBox(bb, lineEffect, ScèneJeu.GraphicsDevice, ScèneJeu.CaméraJeu.Vue, ScèneJeu.CaméraJeu.Projection);
                }
            }
            base.Draw(gameTime);
        }

        protected virtual void GérérCouleurCharm(ref BasicEffect effet)
        {
        }

        protected virtual void GérerSurlignage(ref BasicEffect effet)
        {
        }

        private void GérerClavier()
        {
            if (ScèneJeu.GestionInput3D.EstNouvelleTouche(Keys.B))
            {
                DrawBoxes ^= true;
            }
        }

        public virtual Matrix GetMonde()
        {
            return Monde;
        }

        private void UpdateWorld()
        {
            Monde = Matrix.Identity * Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y + RotationOffset.Y, Rotation.X + RotationOffset.X, Rotation.Z + RotationOffset.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        // Create/update the BoxList and BoxDrawList
        private void CreateBoxList()
        {
            BoxList.Clear();
            BoxDrawList.Clear();

            // on créé un tableau contenant la liste des sommets du modèles
            ModelMeshPart portionMaillage = Modèle.Meshes[0].MeshParts[0];
            int tailleSommetEnFloat = portionMaillage.VertexBuffer.VertexDeclaration.VertexStride / sizeof(float);
            int tailleTamponEnFloat = portionMaillage.VertexBuffer.VertexCount * tailleSommetEnFloat;

            // On va chercher le contenu du tampon de sommets (vertex buffer) en float pour que cela fonctionne 
            // avec les différents formats de sommets (VertexPositionNormalTexture, VertexPositionTexture, VertexPositionColor...)
            float[] sommetsDuModèles = new float[tailleTamponEnFloat];
            portionMaillage.VertexBuffer.GetData<float>(sommetsDuModèles);
            int début = 0;
            //On crée une boîte de collision par maillage (un modèle contient de 1 à N maillages)
            foreach (ModelMesh maillage in Modèle.Meshes)
            {
                //On crée la boîte de collision (BoundingBox) correspondant à une partie du modèle et on l'ajoute à la liste des boîtes de collision
                BoundingBox boîteCollision = CalculerBoundingBox(maillage, sommetsDuModèles, tailleSommetEnFloat, ref début);
                BoxList.Add(boîteCollision);
                BoxDrawList.Add(CreateBoundingBoxBuffers(boîteCollision, Jeu.GraphicsDevice));
            }
        }

        // Calcule et retourne BoundingBox
        private BoundingBox CalculerBoundingBox(ModelMesh maillage, float[] sommetsDuModèle, int tailleSommetEnFloat, ref int début)
        {
            int nbSommets = 0;
            foreach (ModelMeshPart portionDeMaillage in maillage.MeshParts)
            {
                nbSommets += portionDeMaillage.NumVertices;
            }
            Vector3 maxMaillage = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 minMaillage = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            int index = 0;
            Vector3[] positionSommets = new Vector3[nbSommets];
            int fin = début + nbSommets * tailleSommetEnFloat;
            // on parcourt la portion de la liste des sommets correspondant à cette partie du modèle
            for (int indiceSommet = début; indiceSommet < fin && indiceSommet < sommetsDuModèle.Length; indiceSommet += tailleSommetEnFloat)
            {
                Vector3 sommet = new Vector3(sommetsDuModèle[indiceSommet], sommetsDuModèle[indiceSommet + 1], sommetsDuModèle[indiceSommet + 2]);
                positionSommets[index] = sommet;
                minMaillage = Vector3.Min(sommet, minMaillage);
                maxMaillage = Vector3.Max(sommet, maxMaillage);
                ++index;
            }
            début = fin;
            BoundingBox boîte = new BoundingBox(minMaillage, maxMaillage);
            Vector3[] listeDesCoins = boîte.GetCorners();
            Matrix mondeLocal = maillage.ParentBone.Transform;
            mondeLocal *= Matrix.CreateScale(Échelle * ÉchelleBox);
            mondeLocal *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            mondeLocal *= Matrix.CreateTranslation(Position);
            Vector3.Transform(listeDesCoins, ref mondeLocal, listeDesCoins);
            boîte = BoundingBox.CreateFromPoints(listeDesCoins);
            return boîte;
        }

        // Draw a single BoundingBox
        protected void DrawBoundingBox(BoundingBoxBuffers buffers, BasicEffect effect, GraphicsDevice graphicsDevice, Matrix view, Matrix projection)
        {
            graphicsDevice.SetVertexBuffer(buffers.Vertices);
            graphicsDevice.Indices = buffers.Indices;

            effect.World = Matrix.Identity;
            effect.View = view;
            effect.Projection = projection;

            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList, 0, 0,
                    buffers.VertexCount, 0, buffers.PrimitiveCount);
            }
        }

        // Create and return the (drawable) bounding box
        private BoundingBoxBuffers CreateBoundingBoxBuffers(BoundingBox boundingBox, GraphicsDevice graphicsDevice)
        {
            BoundingBoxBuffers boundingBoxBuffers = new BoundingBoxBuffers();

            boundingBoxBuffers.PrimitiveCount = 24;
            boundingBoxBuffers.VertexCount = 48;

            VertexBuffer vertexBuffer = new VertexBuffer(graphicsDevice,
                typeof(VertexPositionColor), boundingBoxBuffers.VertexCount,
                BufferUsage.WriteOnly);
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();

            const float ratio = 5.0f;

            Vector3 xOffset = new Vector3((boundingBox.Max.X - boundingBox.Min.X) / ratio, 0, 0);
            Vector3 yOffset = new Vector3(0, (boundingBox.Max.Y - boundingBox.Min.Y) / ratio, 0);
            Vector3 zOffset = new Vector3(0, 0, (boundingBox.Max.Z - boundingBox.Min.Z) / ratio);
            Vector3[] corners = boundingBox.GetCorners();

            // Corner 1.
            AddVertex(vertices, corners[0]);
            AddVertex(vertices, corners[0] + xOffset);
            AddVertex(vertices, corners[0]);
            AddVertex(vertices, corners[0] - yOffset);
            AddVertex(vertices, corners[0]);
            AddVertex(vertices, corners[0] - zOffset);

            // Corner 2.
            AddVertex(vertices, corners[1]);
            AddVertex(vertices, corners[1] - xOffset);
            AddVertex(vertices, corners[1]);
            AddVertex(vertices, corners[1] - yOffset);
            AddVertex(vertices, corners[1]);
            AddVertex(vertices, corners[1] - zOffset);

            // Corner 3.
            AddVertex(vertices, corners[2]);
            AddVertex(vertices, corners[2] - xOffset);
            AddVertex(vertices, corners[2]);
            AddVertex(vertices, corners[2] + yOffset);
            AddVertex(vertices, corners[2]);
            AddVertex(vertices, corners[2] - zOffset);

            // Corner 4.
            AddVertex(vertices, corners[3]);
            AddVertex(vertices, corners[3] + xOffset);
            AddVertex(vertices, corners[3]);
            AddVertex(vertices, corners[3] + yOffset);
            AddVertex(vertices, corners[3]);
            AddVertex(vertices, corners[3] - zOffset);

            // Corner 5.
            AddVertex(vertices, corners[4]);
            AddVertex(vertices, corners[4] + xOffset);
            AddVertex(vertices, corners[4]);
            AddVertex(vertices, corners[4] - yOffset);
            AddVertex(vertices, corners[4]);
            AddVertex(vertices, corners[4] + zOffset);

            // Corner 6.
            AddVertex(vertices, corners[5]);
            AddVertex(vertices, corners[5] - xOffset);
            AddVertex(vertices, corners[5]);
            AddVertex(vertices, corners[5] - yOffset);
            AddVertex(vertices, corners[5]);
            AddVertex(vertices, corners[5] + zOffset);

            // Corner 7.
            AddVertex(vertices, corners[6]);
            AddVertex(vertices, corners[6] - xOffset);
            AddVertex(vertices, corners[6]);
            AddVertex(vertices, corners[6] + yOffset);
            AddVertex(vertices, corners[6]);
            AddVertex(vertices, corners[6] + zOffset);

            // Corner 8.
            AddVertex(vertices, corners[7]);
            AddVertex(vertices, corners[7] + xOffset);
            AddVertex(vertices, corners[7]);
            AddVertex(vertices, corners[7] + yOffset);
            AddVertex(vertices, corners[7]);
            AddVertex(vertices, corners[7] + zOffset);

            vertexBuffer.SetData(vertices.ToArray());
            boundingBoxBuffers.Vertices = vertexBuffer;

            IndexBuffer indexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, boundingBoxBuffers.VertexCount,
                BufferUsage.WriteOnly);
            indexBuffer.SetData(Enumerable.Range(0, boundingBoxBuffers.VertexCount).Select(i => (short)i).ToArray());
            boundingBoxBuffers.Indices = indexBuffer;

            return boundingBoxBuffers;
        }

        private static void AddVertex(List<VertexPositionColor> vertices, Vector3 position)
        {
            vertices.Add(new VertexPositionColor(position, Color.White));
        }
    }
}
