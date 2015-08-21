using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class Doodad : ObjetDeBasePhysique
    {
        string NomTexture { get; set; }
        Texture2D Texture { get; set; }
        BlendState OldBlendState { get; set; }
        BlendState NewBlendState { get; set; }
        DepthStencilState OldStencilState { get; set; }
        DepthStencilState NewStencilState { get; set; }
        public bool ToRemove { get; private set; }
        bool UseAlphaBlend { get; set; }

        public Doodad(RPG jeu, ScèneDeJeu scènejeu, string nomModèle, string nomtexture, float échelleInitiale, float échelleBox, Vector2 positionInitiale, Vector3 rotationInitiale, bool useAlphaBlend)
            : base(jeu, scènejeu, nomModèle, échelleInitiale, échelleBox, rotationInitiale, Vector3.Zero, new Vector3(positionInitiale.X, scènejeu.MapManager.CalculerHauteurPoint(positionInitiale), positionInitiale.Y))
        {
            NomTexture = nomtexture;
            UseAlphaBlend = useAlphaBlend;
        }

        protected override void LoadContent()
        {
            OldBlendState = Jeu.GraphicsDevice.BlendState;
            if (UseAlphaBlend)
            {
                NewBlendState = BlendState.AlphaBlend;
            }
            else
            {
                NewBlendState = BlendState.Opaque;
            }
            Texture = ScèneJeu.GestionnaireDeTextures.Find(NomTexture);
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            Jeu.GraphicsDevice.BlendState = NewBlendState;

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
                        effect.Texture = Texture; //not sure if works
                        effect.EnableDefaultLighting();

                        effect.SpecularColor = new Vector3(0.25f);
                        effect.SpecularPower = 16;
                    }

                    mesh.Draw();
                }
            }
            else
            {
                foreach (ModelMesh maille in Modèle.Meshes)
                {
                    Matrix mondeLocal = TransformationsModèle[maille.ParentBone.Index] * GetMonde();
                    foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                    {
                        BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                        effet.EnableDefaultLighting();
                        effet.Projection = ScèneJeu.CaméraJeu.Projection;
                        effet.View = ScèneJeu.CaméraJeu.Vue;
                        effet.World = mondeLocal;
                        effet.TextureEnabled = true;
                        effet.Texture = Texture;

                    }
                    maille.Draw();
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
            }

            Jeu.GraphicsDevice.BlendState = OldBlendState;
        }

    }

    public class DoodadManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public RPG Jeu { get; private set; }
        public ScèneDeJeu ScèneJeu { get; private set; }

        public List<Doodad> ListeDoodads { get; private set; }

        public DoodadManager(RPG jeu, ScèneDeJeu scèneJeu)
            : base(jeu)
        {
            Jeu = jeu;
            ScèneJeu = scèneJeu;
        }

        public override void Initialize()
        {
            ListeDoodads = new List<Doodad>();

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            CheckForDoodadsToRemove();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawDoodads(gameTime);
            base.Draw(gameTime);
        }

        public Doodad CréerDoodad(string nomModèle, string texture, float échelleInitiale, float échelleBox, Vector2 positionInitiale, Vector3 rotationInitiale, bool useAlphaBlend)
        {
            Doodad newDoodad = new Doodad(Jeu, ScèneJeu, nomModèle, texture, échelleInitiale, échelleBox, positionInitiale, rotationInitiale, useAlphaBlend);
            ListeDoodads.Add(newDoodad);
            newDoodad.Initialize();
            return newDoodad;
        }

        private void CheckForDoodadsToRemove()
        {
            for (int i = ListeDoodads.Count - 1; i >= 0; --i)
            {
                if (ListeDoodads[i].ToRemove)
                {
                    ListeDoodads[i].BoxList.Clear();
                    ListeDoodads[i].BoxDrawList.Clear();
                    ListeDoodads.RemoveAt(i);
                }
            }
        }

        private void DrawDoodads(GameTime gameTime)
        {
            foreach (Doodad doodad in ListeDoodads)
            {
                if (Vector3.Distance(doodad.Position, ScèneJeu.BaldorLeBrave.Position) <= Caméra.DISTANCE_PLAN_ÉLOIGNÉ)
                {
                    doodad.Draw(gameTime);
                }
            }
        }
    }
}
