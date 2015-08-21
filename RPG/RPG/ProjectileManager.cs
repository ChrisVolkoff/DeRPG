using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class ProjectileManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public RPG Jeu { get; private set; }
        public ScèneDeJeu ScèneJeu { get; private set; }

        public List<ProjectileBalistique> ListeProjectiles { get; private set; }
        public int CptProj { get; private set; }
        public string ModelName { get; private set; }
        public float ModelScale { get; private set; }

        public float TempsÉcouléDepuisMAJ { get; private set; }

        public bool IsSlowing { get { return ScèneJeu.BaldorLeBrave.IsSlowingProj; } }

        public ProjectileManager(RPG jeu, ScèneDeJeu scèneJeu, string nomModel, float modelScale)
            : base(jeu)
        {
            Jeu = jeu;
            ScèneJeu = scèneJeu;
            ModelName = nomModel;
            ModelScale = modelScale;
        }

        public override void Initialize()
        {
            ListeProjectiles = new List<ProjectileBalistique>();
            CptProj = 1;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            CheckForProjectilesToRemove();
            UpdateProjectiles(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawProjectiles(gameTime);
            base.Draw(gameTime);
        }

        public void CréerProjectile(Combattant combattant, Vector3 posCible, int damage, float PortéeMax)
        {
            ProjectileBalistique newProj = new ProjectileBalistique(Jeu, ScèneJeu, combattant, damage, CptProj, combattant.Position, posCible, PortéeMax, ModelName, ModelScale, Vector3.Zero);
            ListeProjectiles.Add(newProj);
            newProj.Initialize();
            ++CptProj;

            if (combattant is Héros)
            {
                Soundtrack.StartSoundCue("proj_cast");
            }
        }

        private void CheckForProjectilesToRemove()
        {
            for (int i = ListeProjectiles.Count - 1; i >= 0; --i)
            {
                if (ListeProjectiles[i].ToRemove)
                {
                    ListeProjectiles.RemoveAt(i);
                    Soundtrack.StartSoundCue("proj_explosion");
                }
            }
        }

        private void UpdateProjectiles(GameTime gameTime)
        {
            foreach (ProjectileBalistique proj in ListeProjectiles)
            {
                if (!(proj.Propriétaire is Héros)) // Updates the slowing property for the affected people (!Héros)
                {
                    proj.IsSlowing = IsSlowing;
                }
                proj.Update(gameTime);
            }
        }

        private void DrawProjectiles(GameTime gameTime)
        {
            foreach (ProjectileBalistique proj in ListeProjectiles)
            {
                proj.Draw(gameTime);
            }
        }

        // Checks if a projectile is active using his ID
        public bool IsIDActive(int ID)
        {
            bool result = ListeProjectiles.Exists(delegate(ProjectileBalistique proj)
            {
                return proj.ID == ID;
            }
            );
            return result;
        }

        public ProjectileBalistique GetProjectileWithID(int id)
        {
            return ListeProjectiles.Find(delegate(ProjectileBalistique proj)
            {
                return proj.ID == id;
            }
            );
        }
    }
}
