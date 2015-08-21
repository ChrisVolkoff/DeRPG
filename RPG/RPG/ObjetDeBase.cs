using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
    public class ObjetDeBase : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected RPG Jeu { get; private set; }
        protected ScèneDeJeu ScèneJeu { get; private set; }
        private string NomModèle { get; set; }
        public float Échelle { get; protected set; }
        public Vector3 Rotation { get; protected set; }
        public Vector3 Position { get; protected set; }

        protected Model Modèle { get; private set; }
        protected Matrix[] TransformationsModèle { get; private set; }
        protected Matrix Monde { get; set; }

        public ObjetDeBase(RPG jeu, ScèneDeJeu scèneJeu, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
            : base(jeu)
        {
            Jeu = jeu;
            ScèneJeu = scèneJeu;
            NomModèle = nomModèle;
            Position = positionInitiale;
            Échelle = échelleInitiale;
            Rotation = rotationInitiale;
        }

        public override void Initialize()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);

            base.Initialize();
        }
        protected override void LoadContent()
        {
            Modèle = Jeu.GestionnaireDeModèles.Find(NomModèle);
            TransformationsModèle = new Matrix[Modèle.Bones.Count];
            Modèle.CopyAbsoluteBoneTransformsTo(TransformationsModèle);
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (Modèle == null) throw new Exception("Modèle est null");
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
