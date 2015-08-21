using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class ProjectileManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public RPG Jeu { get; private set; }
        public Sc�neDeJeu Sc�neJeu { get; private set; }

        public List<ProjectileBalistique> ListeProjectiles { get; private set; }
        public int CptProj { get; private set; }
        public string ModelName { get; private set; }
        public float ModelScale { get; private set; }

        public float Temps�coul�DepuisMAJ { get; private set; }

        public bool IsSlowing { get { return Sc�neJeu.BaldorLeBrave.IsSlowingProj; } }

        public ProjectileManager(RPG jeu, Sc�neDeJeu sc�neJeu, string nomModel, float modelScale)
            : base(jeu)
        {
            Jeu = jeu;
            Sc�neJeu = sc�neJeu;
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

        public void Cr�erProjectile(Combattant combattant, Vector3 posCible, int damage, float Port�eMax)
        {
            ProjectileBalistique newProj = new ProjectileBalistique(Jeu, Sc�neJeu, combattant, damage, CptProj, combattant.Position, posCible, Port�eMax, ModelName, ModelScale, Vector3.Zero);
            ListeProjectiles.Add(newProj);
            newProj.Initialize();
            ++CptProj;

            if (combattant is H�ros)
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
                if (!(proj.Propri�taire is H�ros)) // Updates the slowing property for the affected people (!H�ros)
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
