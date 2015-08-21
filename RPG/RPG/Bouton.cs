using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class Bouton : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected SpriteBatch GestionSprites { get; set; }
        protected RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        protected RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        protected InputManager GestionInput { get; set; }
        protected RPG Jeu { get; set; }
        protected GestionnaireDeScènes SceneManager { get; set; }

        public Vector2 Position { get; private set; }
        protected string NomFont { get; set; }
        protected SpriteFont Font { get; set; }
        protected Color CurrentColor { get; set; }
        protected Color DefaultColor { get; set; }
        protected Color AltColor { get; set; }
        protected Rectangle DetectionRectangle { get; set; }

        protected float TempsÉcouléDepuisMAJ { get; set; }

        public Bouton(RPG jeu, Vector2 position, string nomFont, Color defaultColor, Color altColor, GestionnaireDeScènes sceneManager)
            : base(jeu)
        {
            Jeu = jeu;
            SceneManager = sceneManager;
            Position = position;
            NomFont = nomFont;
            DefaultColor = defaultColor;
            AltColor = altColor;

            GetServices();
        }

        public override void Initialize()
        {
            CurrentColor = DefaultColor;
            TempsÉcouléDepuisMAJ = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Font = GestionnaireDeFonts.Find(NomFont);
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected bool IsMouseHover()
        {
            Rectangle RectangleSouris = new Rectangle((int)GestionInput.GetPositionSouris().X, (int)GestionInput.GetPositionSouris().Y, 10, 10);
            return RectangleSouris.Intersects(DetectionRectangle);
        }

        private void GetServices()
        {
            GestionSprites = (SpriteBatch)Jeu.Services.GetService(typeof(SpriteBatch));
            GestionnaireDeFonts = (RessourcesManager<SpriteFont>)Jeu.Services.GetService(typeof(RessourcesManager<SpriteFont>));
            GestionnaireDeTextures = (RessourcesManager<Texture2D>)Jeu.Services.GetService(typeof(RessourcesManager<Texture2D>));
            GestionInput = (InputManager)Jeu.Services.GetService(typeof(InputManager));
        }
    }
}
