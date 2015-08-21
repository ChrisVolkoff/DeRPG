using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace AtelierXNA
{
    public delegate void ButtonClickedEventHandler(BoutonTextuel sender);

    public class BoutonTextuel : Bouton
    {
        // Constantes de scale
        const float DEFAULT_SCALE = 1.0f;
        const float ALT_SCALE = 1.15f;

        public string Texte { get;  set; }
        public Scènes DestinationScene { get; set; }

        public Vector2 Origin { get; private set; }
        public float Scale { get; private set; }

        public event ButtonClickedEventHandler Clicked;

        /// <param name="oneTimeButton">Définit si le bouton disparait après avoir été cliqué une fois</param>
        public BoutonTextuel(RPG jeu, Vector2 position, string texte, string nomFont, Color defaultColor, Color altColor, Scènes destinationScene, GestionnaireDeScènes sceneManager)
            : base(jeu, position, nomFont, defaultColor, altColor, sceneManager)
        {
            Texte = texte;
            DestinationScene = destinationScene;
        }

        public override void Initialize()
        {
            Scale = DEFAULT_SCALE;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            DetectionRectangle = new Rectangle((int)Position.X - (int)Font.MeasureString(Texte).X / 2, (int)Position.Y - (int)Font.MeasureString(Texte).Y / 2, (int)Font.MeasureString(Texte).X, (int)Font.MeasureString(Texte).Y);
            Origin = new Vector2(DetectionRectangle.Width / 2, DetectionRectangle.Height / 2);
        }

        public override void Update(GameTime gameTime)
        {
            if (IsMouseHover())
            {
                CurrentColor = AltColor;
                Scale = ALT_SCALE;

                if (GestionInput.EstNouveauClicGauche())
                {
                    Soundtrack.StartSoundCue("button_click");

                    SceneManager.ChangerDeScène(DestinationScene);
                    OnClick();
                }
            }
            else
            {
                CurrentColor = DefaultColor;
                Scale = DEFAULT_SCALE;
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            GestionSprites.Begin();
            GestionSprites.DrawString(Font, Texte, Position, CurrentColor, 0f, Origin, Scale, SpriteEffects.None, 0f);
            GestionSprites.End();
            base.Draw(gameTime);
        }

        protected virtual void OnClick()
        {
           if (Clicked != null)
              Clicked(this);
        }
    }
}
