using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace AtelierXNA
{
    public class BoutonR�glage : Bouton
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

        // Valeur de r�glage (0 � 1)
        public float Valeur { get { return ((PositionBouton.X - (Position.X + ((float)BUTTON_WIDTH / 2))) / ((Position.X + (float)BUTTON_BACKGROUND_WIDTH - (((float)BUTTON_WIDTH / 2) + (float)BUTTON_WIDTH)) - (Position.X + ((float)BUTTON_WIDTH / 2)))); } }

        private Vector2 PositionBackground { get; set; }
        private Vector2 PositionBouton { get; set; }
        private Vector2 PositionTexte { get; set; }
        private Vector2 PositionTexteR�sultat { get; set; }
        private Rectangle ButtonRectangle { get { return new Rectangle((int)PositionBouton.X, (int)PositionBouton.Y, BUTTON_WIDTH, BUTTON_HEIGHT); } }

        public string TexteR�sultat { get { return ((int)(Valeur * 100)).ToString() + "%"; } }

        public BoutonR�glage(RPG jeu, int valeurInitiale, Vector2 position, string texte, string nomFont, Color defaultColor, Color altColor, GestionnaireDeSc�nes sceneManager)
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
            PositionTexteR�sultat = new Vector2(Position.X + BUTTON_BACKGROUND_WIDTH, PositionTexte.Y);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsMouseHover())
            {
                CurrentColor = AltColor;

                if (GestionInput.�tatSouris.LeftButton == ButtonState.Pressed)
                {
                    PositionBouton = new Vector2(MathHelper.Clamp(GestionInput.�tatSouris.X - (BUTTON_WIDTH / 2), Position.X + (BUTTON_WIDTH / 2), Position.X + BUTTON_BACKGROUND_WIDTH - ((BUTTON_WIDTH / 2) + BUTTON_WIDTH )), PositionBouton.Y);
                }
            }
            else
            {
                CurrentColor = DefaultColor;
            }

            if (GestionInput.�tatClavier.IsKeyDown(Keys.S))
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
            GestionSprites.DrawString(Font, TexteR�sultat, PositionTexteR�sultat, Color.Black, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            GestionSprites.End();
            base.Draw(gameTime);
        }

        private int D�placementSourisX()
        {
            return (int)GestionInput.GetPositionSouris().X - GestionInput.Ancien�tatSouris.X;
        }
    }
}
