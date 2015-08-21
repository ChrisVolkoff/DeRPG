using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class BoutonBool : Bouton
    {
        // Constantes dimension des boutons
        const int BUTTON_BACKGROUND_HEIGHT = 50;
        const int BUTTON_BACKGROUND_WIDTH = 50;
        const int MARK_HEIGHT = 60;
        const int MARK_WIDTH = 40;

        private Texture2D ImageFond { get; set; }
        private string Texte { get; set; }
        private char Car { get; set; }

        // Valeur de réglage (true or false)
        bool _valeur;
        public bool Valeur
        {
            get { return _valeur; }
            private set
            {
                Jeu.PériphériqueGraphique.ToggleFullScreen();
                _valeur = value;
            }
        }

        private Vector2 PositionBackground { get; set; }
        private Vector2 PositionTexte { get; set; }
        private Vector2 PositionTexteRésultat { get; set; }
        private Vector2 ScaleMarqueur { get; set; }

        public string TexteRésultat { get { return Valeur.ToString(); } }
        public string Marqueur
        {
            get
            {
                string retour;
                if (Valeur)
                {
                    retour = Car.ToString();
                }
                else
                {
                    retour = "";
                }
                return retour;
            }
        }

        public BoutonBool(RPG jeu, Vector2 position, char car, string texte, string nomFont, Color defaultColor, Color altColor, GestionnaireDeScènes sceneManager)
            : base(jeu, position, nomFont, defaultColor, altColor, sceneManager)
        {
            Car = car;
            Texte = texte;
        }

        public override void Initialize()
        {
            Valeur = true;
            DetectionRectangle = new Rectangle((int)Position.X, (int)Position.Y, BUTTON_BACKGROUND_WIDTH, BUTTON_BACKGROUND_HEIGHT);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ImageFond = GestionnaireDeTextures.Find("Bouton_fond");
            base.LoadContent();
            PositionTexte = new Vector2(Position.X - Font.MeasureString(Texte).X, (Position.Y + (BUTTON_BACKGROUND_HEIGHT / 2)) - (Font.MeasureString(Texte).Y / 2));
            ScaleMarqueur = new Vector2(DetectionRectangle.Width / Font.MeasureString(Marqueur).X, DetectionRectangle.Height / Font.MeasureString(Marqueur).Y);
            PositionTexteRésultat = new Vector2(Position.X + BUTTON_BACKGROUND_WIDTH, PositionTexte.Y);
        }

        public override void Update(GameTime gameTime)
        {
            TempsÉcouléDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TempsÉcouléDepuisMAJ >= Jeu.INTERVALLE_MAJ)
            {
                UpdateButton();
                TempsÉcouléDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        private void UpdateButton()
        {
            if (IsMouseHover())
            {
                CurrentColor = AltColor;
                if (GestionInput.EstNouveauClicGauche())
                {
                    Soundtrack.StartSoundCue("button_click");

                    Valeur ^= true;
                }
            }
            else
            {
                CurrentColor = DefaultColor;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();

            GestionSprites.DrawString(Font, Texte, PositionTexte, CurrentColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            GestionSprites.Draw(ImageFond, DetectionRectangle, Color.White);
            GestionSprites.DrawString(Font, Marqueur, Position, Color.Black, 0f, Vector2.Zero, ScaleMarqueur, SpriteEffects.None, 0f);
            GestionSprites.DrawString(Font, TexteRésultat, PositionTexteRésultat, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

            GestionSprites.End();
            base.Draw(gameTime);
        }
    }
}
