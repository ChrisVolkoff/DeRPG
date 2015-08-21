using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace AtelierXNA
{
    public class BoutonRéglage : Bouton
    {
        // Constantes des boutons
        const int BUTTON_BACKGROUND_HEIGHT = 40;
        const int BUTTON_BACKGROUND_WIDTH = 400;
        const int BUTTON_HEIGHT = 60;
        const int BUTTON_WIDTH = 40;
        const string BUTTON_IMAGE = "Bouton_hover";
        const string BUTTON_IMAGE_BACKGROUND = "Bouton_fond";

        private int ValeurInitiale { get; set; }
        private Texture2D ImageBouton { get; set; }
        private Texture2D ImageFond { get; set; }
        private string Texte { get; set; }

        // Valeur de réglage (0 à 1)
        public float Valeur { get { return ((PositionBouton.X - (Position.X + ((float)BUTTON_WIDTH / 2))) / ((Position.X + (float)BUTTON_BACKGROUND_WIDTH - (((float)BUTTON_WIDTH / 2) + (float)BUTTON_WIDTH)) - (Position.X + ((float)BUTTON_WIDTH / 2)))); } }

        private Vector2 PositionBackground { get; set; }
        private Vector2 PositionBouton { get; set; }
        private Vector2 PositionTexte { get; set; }
        private Vector2 PositionTexteRésultat { get; set; }
        private Rectangle ButtonRectangle { get { return new Rectangle((int)PositionBouton.X, (int)PositionBouton.Y, BUTTON_WIDTH, BUTTON_HEIGHT); } }

        public string TexteRésultat { get { return ((int)(Valeur * 100)).ToString() + "%"; } }

        public BoutonRéglage(RPG jeu, int valeurInitiale, Vector2 position, string texte, string nomFont, Color defaultColor, Color altColor, GestionnaireDeScènes sceneManager)
            : base(jeu, position, nomFont, defaultColor, altColor, sceneManager)
        {
            ValeurInitiale = (int)MathHelper.Clamp(valeurInitiale, 0, 100);
            Texte = texte;
        }

        public override void Initialize()
        {
            PositionBouton = new Vector2(Position.X + (BUTTON_WIDTH / 2) + (((Position.X + (float)BUTTON_BACKGROUND_WIDTH - (((float)BUTTON_WIDTH / 2) + (float)BUTTON_WIDTH)) - (Position.X + ((float)BUTTON_WIDTH / 2))) * (ValeurInitiale / 100f)), Position.Y + (BUTTON_BACKGROUND_HEIGHT / 2) - (BUTTON_HEIGHT / 2));
            DetectionRectangle = new Rectangle((int)Position.X, (int)Position.Y, BUTTON_BACKGROUND_WIDTH, BUTTON_BACKGROUND_HEIGHT);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            ImageBouton = GestionnaireDeTextures.Find(BUTTON_IMAGE);
            ImageFond = GestionnaireDeTextures.Find(BUTTON_IMAGE_BACKGROUND);
            base.LoadContent();
            PositionTexte = new Vector2(Position.X - Font.MeasureString(Texte).X, (Position.Y + (BUTTON_BACKGROUND_HEIGHT / 2)) - (Font.MeasureString(Texte).Y / 2));
            PositionTexteRésultat = new Vector2(Position.X + BUTTON_BACKGROUND_WIDTH, PositionTexte.Y);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsMouseHover())
            {
                CurrentColor = AltColor;

                if (GestionInput.ÉtatSouris.LeftButton == ButtonState.Pressed)
                {
                    PositionBouton = new Vector2(MathHelper.Clamp(GestionInput.ÉtatSouris.X - (BUTTON_WIDTH / 2), Position.X + (BUTTON_WIDTH / 2), Position.X + BUTTON_BACKGROUND_WIDTH - ((BUTTON_WIDTH / 2) + BUTTON_WIDTH )), PositionBouton.Y);
                }
            }
            else
            {
                CurrentColor = DefaultColor;
            }

            if (GestionInput.ÉtatClavier.IsKeyDown(Keys.S))
            {
                PositionBouton = new Vector2(((BUTTON_BACKGROUND_WIDTH / 2) + Position.X - BUTTON_WIDTH / 2) + (float)(Math.Sin((double)gameTime.TotalGameTime.TotalSeconds) * ((BUTTON_BACKGROUND_WIDTH / 2) - BUTTON_WIDTH)), PositionBouton.Y);
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.DrawString(Font, Texte, PositionTexte, CurrentColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            GestionSprites.Draw(ImageFond, DetectionRectangle, Color.White);
            GestionSprites.Draw(ImageBouton, ButtonRectangle, Color.White);
            GestionSprites.DrawString(Font, TexteRésultat, PositionTexteRésultat, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            GestionSprites.End();
            base.Draw(gameTime);
        }

        private int DéplacementSourisX()
        {
            return (int)GestionInput.GetPositionSouris().X - GestionInput.AncienÉtatSouris.X;
        }
    }
}
