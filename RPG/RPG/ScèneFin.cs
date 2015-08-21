using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public class ScèneFin : ScèneMenu
    {
        const string ESPACES = "                  ";
        
        public ScèneFin(RPG jeu, string nomImage, GestionnaireDeScènes scènesMgr)
            : base(jeu, nomImage, scènesMgr)
        {
        }

        public override void Initialize()
        {
            Vector2 position = new Vector2(640, 605);
            BoutonTextuel bouton = new BoutonTextuel(Jeu, position, ESPACES, "Arial", Color.Black, Color.Black, Scènes.MenuPrincipal, SceneManager);
            ListeDesÉléments.Add(bouton);
            base.Initialize();
        }

        // Appelée lorsque la scène est activée (Initialize, mais à chaque activation de la scène)
        public override void Activate()
        {

            base.Activate();
        }

        public override void Update(GameTime gameTime)
        {
            // Easter egg!
            if (GestionInput.EstNouveauClicGauche() && (new Rectangle(50, 100, 50, 50).Contains(GestionInput.ÉtatSouris.X, GestionInput.ÉtatSouris.Y)))
            {
                Soundtrack.StartSoundCue("cain_cube");
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}