using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class ScèneEvent : Scène
    {
        const float ZOOM = 0.005f;
        const float PIVOT_ANGLE = 1.8f;
        const float ANGLES_TOURS = 720f;

        private string Message { get; set; }
        private SpriteFont Font { get; set; }
        private Vector2 Origine { get; set; }
        private Vector2 Position { get; set; }
        private Vector2 Dimension { get; set; }
        private Color CouleurMessage { get; set; }
        private Color CouleurBackground { get; set; }

        private float Rotation { get; set; }
        private float Scale { get; set; }
        private float TempsDepuisMAJ { get; set; }

        private bool Quit { get; set; }

        public ScèneEvent(RPG game,string message,GestionnaireDeScènes scèneMgr,Color couleurMessage,Color couleurBackground,bool quit)
            : base(game,scèneMgr)
        {
            Message = message;
            CouleurMessage = couleurMessage;
            CouleurBackground = couleurBackground;
            Quit = quit;
            Position = new Vector2(Jeu.Window.ClientBounds.Width / 2, Jeu.Window.ClientBounds.Height / 2);
        }
        public override void Initialize()
        {
            Rotation = 0;
            Scale = 0.01f;
            TempsDepuisMAJ = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Font = GestionnaireDeFonts.Find("Infinity60");
            Dimension = Font.MeasureString(Message);
            Origine = new Vector2(Dimension.X / 2, Dimension.Y / 2);
            base.LoadContent();
        }
        public override void Update(GameTime gameTime)
        {         
            TempsDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TempsDepuisMAJ > Jeu.INTERVALLE_MAJ)
            {
               if (Rotation < ANGLES_TOURS - PIVOT_ANGLE)
               {
                  TempsDepuisMAJ = 0;
                  Rotation += PIVOT_ANGLE;
                  Scale += ZOOM;
               }
               else
               {
                  TempsDepuisMAJ = 0;
                  Rotation = 0;
                  Scale = 0.01f;
                  if (Quit)
                  {
                     Jeu.Exit();
                  }
                  else
                  {
                     SceneManager.ChangerDeScène(Scènes.Jeu);  
                  }
                                  
               }              
            }
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(CouleurBackground);
            GestionSprites.Begin();
            AfficherMessage();
            GestionSprites.End();
            base.Draw(gameTime);
        }

        private void AfficherMessage()
        {
            Jeu.GestionSprites.DrawString(Font, Message, Position, CouleurMessage,
                                           MathHelper.ToRadians(Rotation), Origine,
                                           Scale, SpriteEffects.None, 0);
        }
    }
}
