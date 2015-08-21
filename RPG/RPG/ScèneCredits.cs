using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
    public class ScèneCredits : ScèneMenu
    {
        const int ALPHA_START = 0;
        const int ALPHA_END = 255;
        const float ALPHA_INCREMENT = 0.5f;
        const int BLACK = 0;
        const float STARTING_POINT_Y = 10f;
        const float SPEED = 50;
        const float ALT_SPEED = 100;
        private float Vitesse { get; set; }

        private string NomFichierTxt { get; set; }
        private string Texte { get; set; }
        private string NomFont { get; set; }
        private SpriteFont Font { get; set; }
        private Color Color { get; set; }

        private float TempsDepuisMAJ { get; set; }

        private float YCoord { get; set; }
        private float XCoord { get; set; }
        private Vector2 PositionTexte { get { return new Vector2(XCoord, YCoord); } }

        private Vector2 StringDimensions { get; set; }
        private Rectangle StringRectangle { get { return new Rectangle((int)PositionTexte.X, (int)PositionTexte.Y, (int)StringDimensions.X, (int)StringDimensions.Y); } }

        private bool CanStartBlackFadeOut { get { return Soundtrack.IsSoundCueStopped; } }
        private float AlphaValue { get; set; }
        private bool CanStartCredits { get { return AlphaValue >= ALPHA_END; } }
        private bool CreditsEnded { get { return (YCoord + StringDimensions.Y) < 0; } }

        public ScèneCredits(RPG jeu, string nomFichierTxt, string nomFont, Color color, string nomImage, GestionnaireDeScènes scènesMgr)
            : base(jeu, nomImage, scènesMgr)
        {
            NomFichierTxt = nomFichierTxt;
            NomFont = nomFont;
            Color = color;
        }

        public override void Initialize()
        {
            YCoord = Jeu.Window.ClientBounds.Height + STARTING_POINT_Y;
            Vitesse = SPEED;

            AlphaValue = ALPHA_START;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Font = GestionnaireDeFonts.Find(NomFont);
            Texte = new StreamReader(NomFichierTxt).ReadToEnd();
            StringDimensions = Font.MeasureString(Texte);
            XCoord = (RectAffichage.Width / 2) - (StringDimensions.X / 2);
            base.LoadContent();
        }

        // Appelée lorsque la scène est activée (Initialize, mais à chaque activation de la scène)
        public override void Activate()
        {
            GestionInput.Enabled = true; //(re)enable the InputManager
            Soundtrack.StartSongCue("m_credits");
            base.Activate();
        }

        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsDepuisMAJ += tempsÉcoulé;
            if (TempsDepuisMAJ > Jeu.INTERVALLE_MAJ)
            {
                if (CanStartBlackFadeOut)
                {
                    AlphaValue += ALPHA_INCREMENT;
                }

                if (CanStartCredits)
                {
                    if (GestionInput.ÉtatClavier.IsKeyDown(Keys.Space))
                    {
                        Vitesse = ALT_SPEED;
                    }
                    else
                    {
                        Vitesse = SPEED;
                    }
                    YCoord -= TempsDepuisMAJ * Vitesse;
                }

                 


                TempsDepuisMAJ = 0;
            }

            if (CreditsEnded)
            {
                SceneManager.ChangerDeScène(Scènes.Fin);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.Draw(ImageMenu, RectAffichage, new Color((byte)MathHelper.Clamp(AlphaValue, ALPHA_START, ALPHA_END), (byte)MathHelper.Clamp(AlphaValue, ALPHA_START, ALPHA_END), (byte)MathHelper.Clamp(AlphaValue, ALPHA_START, ALPHA_END), 255));
            GestionSprites.DrawString(Font, Texte, PositionTexte, Color);
            GestionSprites.End();
        }
    }
}
