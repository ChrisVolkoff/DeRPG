using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class MenuPrincipal : ScèneMenu
    {
        const string LOGO_NAME = "DeRPG";
        private Texture2D ImageLogo { get; set; }

        public MenuPrincipal(RPG jeu, string nomImage, GestionnaireDeScènes scènesMgr)
            : base(jeu, nomImage, scènesMgr)
        {

        }

        // Add() components
        public override void Initialize()
        {
            ListeDesÉléments.Add(new AfficheurFPS(Jeu, "Arial"));
            BoutonTextuel playbutton = new BoutonTextuel(Jeu, new Vector2(350, 450), "Débuter la partie", "TrajanusRomanBold60", Color.Black, Color.Red, Scènes.CharacterSelection, SceneManager);
            ListeDesÉléments.Add(playbutton);
            playbutton.Clicked +=  new ButtonClickedEventHandler(ChangePlayButton);
            ListeDesÉléments.Add(new BoutonTextuel(Jeu, new Vector2(300, 600), "Options", "TrajanusRomanBold60", Color.Black, Color.Red, Scènes.MenuOptions, SceneManager));
            
            Soundtrack.StartSongCue("m_menu");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ImageLogo = GestionnaireDeTextures.Find(LOGO_NAME);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
           if (GestionInput.EstNouvelleTouche(Keys.Escape))
           {
              Jeu.Exit();
           }
            base.Update(gameTime);
        }

        // Appelée lorsque la scène est activée (Initialize, mais à chaque activation de la scène)
        public override void Activate()
        {
            GestionInput.Enabled = true;
            GestionInput.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            //GestionSprites.Begin();
            //GestionSprites.Draw(ImageLogo, new Vector2(10, 10), null,  Color.White, 0f, Vector2.Zero, 0.45f, SpriteEffects.None, 0f);
            //GestionSprites.End();
        }

        private void ChangePlayButton(BoutonTextuel boutonCliqué)
        {
           int i = ListeDesÉléments.IndexOf(boutonCliqué);
           ListeDesÉléments.Remove(boutonCliqué);
           boutonCliqué.Clicked -= new ButtonClickedEventHandler(ChangePlayButton);
           BoutonTextuel boutonresume = new BoutonTextuel(Jeu, new Vector2(300, 450), "Reprendre", "TrajanusRomanBold60", Color.Black, Color.Red, Scènes.Jeu, SceneManager);
           ListeDesÉléments.Insert(i, boutonresume);
           boutonresume.Initialize();
        }
    }
}