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
        protected Sc�neDeJeu Sc�neJeu { get; private set; }
        private string NomMod�le { get; set; }
        public float �chelle { get; protected set; }
        public Vector3 Rotation { get; protected set; }
        public Vector3 RotationOffset { get; protected set; }
        public Vector3 Position { get; protected set; }
        private float Temps�coul�DepuisMAJ { get; set; }
        protected Model Mod�le { get; private set; }
        protected Matrix[] TransformationsMod�le { get; private set; }
        protected Matrix Monde { get; set; }
        float �chelleBox { get; set; }

        protected AnimationPlayer AnimPlayer { get; set; }

        public List<BoundingBox> BoxList { get; private set; }
        // BoundingBox drawing stuff
        public List<BoundingBoxBuffers> BoxDrawList { get; private set; }
        public BasicEffect lineEffect { get; protected set; }
        public bool DrawBoxes { get; set; }

        public ObjetDeBasePhysique(RPG jeu, Sc�neDeJeu sc�neJeu, String nomMod�le, float �chelleInitiale, float �chelleBox, Vector3 rotationInitiale, Vector3 rotationOffset, Vector3 positionInitiale)
            : base(jeu)
        {
            Jeu = jeu;
            Sc�neJeu = sc�neJeu;
            NomMod�le = nomMod�le;
            Position = positionInitiale;
            �chelle = �chelleInitiale;
            �chelleBox = �chelleBox;
            Rotation = new Vector3(rotationInitiale.X, rotationInitiale.Y, rotationInitiale.Z);
            RotationOffset = rotationOffset;
        }

        public override void Initialize()
        {
            DrawBoxes = false;
            Temps�coul�DepuisMAJ = 0;
            Mod�le = Sc�neJeu.GestionnaireDeMod�les.Find(NomMod�le);
            TransformationsMod�le = new Matrix[Mod�le.Bones.Count];
            Mod�le.CopyAbsoluteBoneTransformsTo(TransformationsMod�le);

            UpdateWorld();

            BoxList = new List<BoundingBox>();
            BoxDrawList = new List<BoundingBoxBuffers>();

            CreateBoxList();

            base.Initialize();
        }
        protected override void LoadContent()
        {
            SkinningData skinningData = Mod�le.Tag as SkinningData;

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

            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TimeSpan accelTime = G�rerTimeScale(gameTime);
            Temps�coul�DepuisMAJ += temps�coul�;
            if (Animated)
            {
                AnimPlayer.Update(accelTime, true, Matrix.Identity);
            }
            if (Temps�coul�DepuisMAJ > Jeu.INTERVALLE_MAJ)
            {
                UpdateWorld();
                CreateBoxList();
                Temps�coul�DepuisMAJ = 0;
            }
            G�rerClavier();
            base.Update(gameTime);
        }

        protected virtual TimeSpan G�rerTimeScale(GameTime gameTime)
        {
            return new TimeSpan((int)(gameTime.ElapsedGameTime.Ticks));
        }

        public override void Draw(GameTime gameTime)
        {
            if (Animated)
            {
                Matrix[] bones = AnimPlayer.GetSkinTransforms();

                // Compute camera matrices.
                Matrix view = Sc�neJeu.Cam�raJeu.Vue;

                Matrix projection = Sc�neJeu.Cam�raJeu.Projection;

                // Render the skinned mesh.
                foreach (ModelMesh mesh in Mod�le.Meshes)
                {
                    Matrix mondeLocal = TransformationsMod�le[mesh.ParentBone.Index] * GetMonde();
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
                foreach (ModelMesh maille in Mod�le.Meshes)
                {
                    Vector3[] oldColor = new Vector3[maille.MeshParts.Count];
                    int i = 0;
                    Matrix mondeLocal = TransformationsMod�le[maille.ParentBone.Index] * GetMonde();
                    foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                    {
                        BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                        oldColor[i] = effet.EmissiveColor;
                        effet.EnableDefaultLighting();
                        G�rerSurlignage(ref effet);
                        G�r�rCouleurCharm(ref effet);
                        effet.Projection = Sc�neJeu.Cam�raJeu.Projection;
                        effet.View = Sc�neJeu.Cam�raJeu.Vue;
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
                if (lineEffect == null) // Comme �a on a qu'� le cr�er une seule fois (et on ne peut le cr�er dans le Initialize())
                {
                    lineEffect = new BasicEffect(Sc�neJeu.GraphicsDevice);
                    lineEffect.LightingEnabled = false;
                    lineEffect.TextureEnabled = false;
                    lineEffect.VertexColorEnabled = true;
                }

                foreach (BoundingBoxBuffers bb in BoxDrawList)
                {
                    DrawBoundingBox(bb, lineEffect, Sc�neJeu.GraphicsDevice, Sc�neJeu.Cam�raJeu.Vue, Sc�neJeu.Cam�raJeu.Projection);
                }
            }
            base.Draw(gameTime);
        }

        protected virtual void G�r�rCouleurCharm(ref BasicEffect effet)
        {
        }

        protected virtual void G�rerSurlignage(ref BasicEffect effet)
        {
        }

        private void G�rerClavier()
        {
            if (Sc�neJeu.GestionInput3D.EstNouvelleTouche(Keys.B))
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
            Monde = Matrix.Identity * Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y + RotationOffset.Y, Rotation.X + RotationOffset.X, Rotation.Z + RotationOffset.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        // Create/update the BoxList and BoxDrawList
        private void CreateBoxList()
        {
            BoxList.Clear();
            BoxDrawList.Clear();

            // on cr�� un tableau contenant la liste des sommets du mod�les
            ModelMeshPart portionMaillage = Mod�le.Meshes[0].MeshParts[0];
            int tailleSommetEnFloat = portionMaillage.VertexBuffer.VertexDeclaration.VertexStride / sizeof(float);
            int tailleTamponEnFloat = portionMaillage.VertexBuffer.VertexCount * tailleSommetEnFloat;

            // On va chercher le contenu du tampon de sommets (vertex buffer) en float pour que cela fonctionne 
            // avec les diff�rents formats de sommets (VertexPositionNormalTexture, VertexPositionTexture, VertexPositionColor...)
            float[] sommetsDuMod�les = new float[tailleTamponEnFloat];
            portionMaillage.VertexBuffer.GetData<float>(sommetsDuMod�les);
            int d�but = 0;
            //On cr�e une bo�te de collision par maillage (un mod�le contient de 1 � N maillages)
            foreach (ModelMesh maillage in Mod�le.Meshes)
            {
                //On cr�e la bo�te de collision (BoundingBox) correspondant � une partie du mod�le et on l'ajoute � la liste des bo�tes de collision
                BoundingBox bo�teCollision = CalculerBoundingBox(maillage, sommetsDuMod�les, tailleSommetEnFloat, ref d�but);
                BoxList.Add(bo�teCollision);
                BoxDrawList.Add(CreateBoundingBoxBuffers(bo�teCollision, Jeu.GraphicsDevice));
            }
        }

        // Calcule et retourne BoundingBox
        private BoundingBox CalculerBoundingBox(ModelMesh maillage, float[] sommetsDuMod�le, int tailleSommetEnFloat, ref int d�but)
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
            int fin = d�but + nbSommets * tailleSommetEnFloat;
            // on parcourt la portion de la liste des sommets correspondant � cette partie du mod�le
            for (int indiceSommet = d�but; indiceSommet < fin && indiceSommet < sommetsDuMod�le.Length; indiceSommet += tailleSommetEnFloat)
            {
                Vector3 sommet = new Vector3(sommetsDuMod�le[indiceSommet], sommetsDuMod�le[indiceSommet + 1], sommetsDuMod�le[indiceSommet + 2]);
                positionSommets[index] = sommet;
                minMaillage = Vector3.Min(sommet, minMaillage);
                maxMaillage = Vector3.Max(sommet, maxMaillage);
                ++index;
            }
            d�but = fin;
            BoundingBox bo�te = new BoundingBox(minMaillage, maxMaillage);
            Vector3[] listeDesCoins = bo�te.GetCorners();
            Matrix mondeLocal = maillage.ParentBone.Transform;
            mondeLocal *= Matrix.CreateScale(�chelle * �chelleBox);
            mondeLocal *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            mondeLocal *= Matrix.CreateTranslation(Position);
            Vector3.Transform(listeDesCoins, ref mondeLocal, listeDesCoins);
            bo�te = BoundingBox.CreateFromPoints(listeDesCoins);
            return bo�te;
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
