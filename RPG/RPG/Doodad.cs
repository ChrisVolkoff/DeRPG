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

        public Doodad(RPG jeu, Sc�neDeJeu sc�nejeu, string nomMod�le, string nomtexture, float �chelleInitiale, float �chelleBox, Vector2 positionInitiale, Vector3 rotationInitiale, bool useAlphaBlend)
            : base(jeu, sc�nejeu, nomMod�le, �chelleInitiale, �chelleBox, rotationInitiale, Vector3.Zero, new Vector3(positionInitiale.X, sc�nejeu.MapManager.CalculerHauteurPoint(positionInitiale), positionInitiale.Y))
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
            Texture = Sc�neJeu.GestionnaireDeTextures.Find(NomTexture);
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            Jeu.GraphicsDevice.BlendState = NewBlendState;

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
                foreach (ModelMesh maille in Mod�le.Meshes)
                {
                    Matrix mondeLocal = TransformationsMod�le[maille.ParentBone.Index] * GetMonde();
                    foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                    {
                        BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                        effet.EnableDefaultLighting();
                        effet.Projection = Sc�neJeu.Cam�raJeu.Projection;
                        effet.View = Sc�neJeu.Cam�raJeu.Vue;
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
            }

            Jeu.GraphicsDevice.BlendState = OldBlendState;
        }

    }

    public class DoodadManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public RPG Jeu { get; private set; }
        public Sc�neDeJeu Sc�neJeu { get; private set; }

        public List<Doodad> ListeDoodads { get; private set; }

        public DoodadManager(RPG jeu, Sc�neDeJeu sc�neJeu)
            : base(jeu)
        {
            Jeu = jeu;
            Sc�neJeu = sc�neJeu;
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

        public Doodad Cr�erDoodad(string nomMod�le, string texture, float �chelleInitiale, float �chelleBox, Vector2 positionInitiale, Vector3 rotationInitiale, bool useAlphaBlend)
        {
            Doodad newDoodad = new Doodad(Jeu, Sc�neJeu, nomMod�le, texture, �chelleInitiale, �chelleBox, positionInitiale, rotationInitiale, useAlphaBlend);
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
                if (Vector3.Distance(doodad.Position, Sc�neJeu.BaldorLeBrave.Position) <= Cam�ra.DISTANCE_PLAN_�LOIGN�)
                {
                    doodad.Draw(gameTime);
                }
            }
        }
    }
}
