using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
    public class ObjetDeBase : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected RPG Jeu { get; private set; }
        protected Sc�neDeJeu Sc�neJeu { get; private set; }
        private string NomMod�le { get; set; }
        public float �chelle { get; protected set; }
        public Vector3 Rotation { get; protected set; }
        public Vector3 Position { get; protected set; }

        protected Model Mod�le { get; private set; }
        protected Matrix[] TransformationsMod�le { get; private set; }
        protected Matrix Monde { get; set; }

        public ObjetDeBase(RPG jeu, Sc�neDeJeu sc�neJeu, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu)
        {
            Jeu = jeu;
            Sc�neJeu = sc�neJeu;
            NomMod�le = nomMod�le;
            Position = positionInitiale;
            �chelle = �chelleInitiale;
            Rotation = rotationInitiale;
        }

        public override void Initialize()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);

            base.Initialize();
        }
        protected override void LoadContent()
        {
            Mod�le = Jeu.GestionnaireDeMod�les.Find(NomMod�le);
            TransformationsMod�le = new Matrix[Mod�le.Bones.Count];
            Mod�le.CopyAbsoluteBoneTransformsTo(TransformationsMod�le);
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (Mod�le == null) throw new Exception("Mod�le est null");
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
                }
                maille.Draw();
            }
            base.Draw(gameTime);
        }

        public virtual Matrix GetMonde()
        {
            return Monde;
        }
    }
}
