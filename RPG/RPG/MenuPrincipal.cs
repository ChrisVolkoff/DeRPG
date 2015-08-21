using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class MenuPrincipal : Sc�neMenu
    {
        const string LOGO_NAME = "DeRPG";
        private Texture2D ImageLogo { get; set; }

        public MenuPrincipal(RPG jeu, string nomImage, GestionnaireDeSc�nes sc�nesMgr)
            : base(jeu, nomImage, sc�nesMgr)
        {

        }

        // Add() components
        public override void Initialize()
        {
            ListeDes�l�ments.Add(new AfficheurFPS(Jeu, "Arial"));
            BoutonTextuel playbutton = new BoutonTextuel(Jeu, new Vector2(350, 450), "D�buter la partie", "TrajanusRomanBold60", Color.Black, Color.Red, Sc�nes.CharacterSelection, SceneManager);
            ListeDes�l�ments.Add(playbutton);
            playbutton.Clicked +=  new ButtonClickedEventHandler(ChangePlayButton);
            ListeDes�l�ments.Add(new BoutonTextuel(Jeu, new Vector2(300, 600), "Options", "TrajanusRomanBold60", Color.Black, Color.Red, Sc�nes.MenuOptions, SceneManager));
            
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

        // Appel�e lorsque la sc�ne est activ�e (Initialize, mais � chaque activation de la sc�ne)
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

        private void ChangePlayButton(BoutonTextuel boutonCliqu�)
        {
           int i = ListeDes�l�ments.IndexOf(boutonCliqu�);
           ListeDes�l�ments.Remove(boutonCliqu�);
           boutonCliqu�.Clicked -= new ButtonClickedEventHandler(ChangePlayButton);
           BoutonTextuel boutonresume = new BoutonTextuel(Jeu, new Vector2(300, 450), "Reprendre", "TrajanusRomanBold60", Color.Black, Color.Red, Sc�nes.Jeu, SceneManager);
           ListeDes�l�ments.Insert(i, boutonresume);
           boutonresume.Initialize();
        }
    }
}