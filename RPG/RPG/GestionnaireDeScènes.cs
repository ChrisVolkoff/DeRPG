using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
   public enum Scènes { MenuLogo, MenuPrincipal, MenuOptions, CharacterSelection, Jeu, Mort, Credits, Fin }

    public class GestionnaireDeScènes : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private RPG Jeu { get; set; }
        private SpriteBatch SpritesManager { get; set; }
        private RessourcesManager<SpriteFont> FontsManager { get; set; }
        private RessourcesManager<Texture2D> TexturesManager { get; set; }
        private RessourcesManager<Model> ModelsManager { get; set; }
        private InputManager InputManager { get; set; }

        List<Scène> ListeDesScènes { get; set; }

        Scènes ScèneActive_;
        public Scènes ScèneActive
        {
            get { return ScèneActive_; }
            set
            {
                ScèneActive_ = value;
                foreach (Scène uneScène in ListeDesScènes)
                {
                    uneScène.Enabled = false;
                    uneScène.Visible = false;
                }
                ListeDesScènes[(int)ScèneActive_].Activate();
                ListeDesScènes[(int)ScèneActive_].Enabled = true;
                ListeDesScènes[(int)ScèneActive_].Visible = true;

            }
        }

        public GestionnaireDeScènes(RPG jeu, SpriteBatch spritesMgr, RessourcesManager<SpriteFont> fontsMgr,
                                      RessourcesManager<Texture2D> texturesMgr, RessourcesManager<Model> modelsMgr, InputManager inputMgr)
            : base(jeu)
        {
            Jeu = jeu;
            SpritesManager = spritesMgr;
            FontsManager = fontsMgr;
            TexturesManager = texturesMgr;
            ModelsManager = modelsMgr;
            InputManager = inputMgr;

            ListeDesScènes = new List<Scène>();
        }

        public override void Initialize()
        {
            // Création des scènes
            Scène ScèneLogo = new ScèneLogo(Jeu, "DeRPG", this);
            ListeDesScènes.Add(ScèneLogo);
            ScèneLogo.Initialize();

            Scène ScèneMenuPrincipal = new MenuPrincipal(Jeu, "Menu_background2", this);
            ListeDesScènes.Add(ScèneMenuPrincipal);
            ScèneMenuPrincipal.Initialize();

            Scène ScèneMenuOptions = new MenuOptions(Jeu, "Menu_background", this);
            ListeDesScènes.Add(ScèneMenuOptions);
            ScèneMenuOptions.Initialize();

            Scène SelectionPerso = new CharacterSelection(Jeu, "CharacterSelect", this);
            ListeDesScènes.Add(SelectionPerso);
            SelectionPerso.Initialize();

            Scène JeuPrincipal = new ScèneDeJeu(Jeu, this);
            ListeDesScènes.Add(JeuPrincipal);
            JeuPrincipal.Initialize();

            Scène ScèneDeMort = new ScèneEvent(Jeu, "GG you died!", this, Color.Yellow, Color.Black, false);
            ListeDesScènes.Add(ScèneDeMort);
            ScèneDeMort.Initialize();

            Scène ScèneCredits = new ScèneCredits(Jeu, "Content/Text/Credits.txt", "TrajanusRoman48", Color.WhiteSmoke, "CreditScreen", this);
            ListeDesScènes.Add(ScèneCredits);
            ScèneCredits.Initialize();

            Scène ScèneFin = new ScèneFin(Jeu, "Congrats", this);
            ListeDesScènes.Add(ScèneFin);
            ScèneFin.Initialize();

            // ScèneActive
            ScèneActive = Scènes.MenuLogo;

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            GérerClavier();

            ListeDesScènes[(int)ScèneActive].Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ListeDesScènes[(int)ScèneActive].Draw(gameTime);
            base.Draw(gameTime);
        }

        private void GérerClavier()
        {

        }

        // Fonction appelée par les scènes
        public void ChangerDeScène(Scènes nouvelleScènes)
        {
            ScèneActive = nouvelleScènes;
        }
    }
}