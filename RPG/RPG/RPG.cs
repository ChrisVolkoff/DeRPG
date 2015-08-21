using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using System.Windows.Forms; On ne peut l'utiliser à cause d'une confusion avec Keys d'XNA
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;

namespace AtelierXNA
{
    public class RPG : Microsoft.Xna.Framework.Game
    {
       public float INTERVALLE_MAJ = 0.01f;

        public GraphicsDeviceManager PériphériqueGraphique { get; private set; }
        public SpriteBatch GestionSprites { get; private set; }
        public InputManager GestionInput { get; private set; }
        public RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        public RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        public RessourcesManager<Model> GestionnaireDeModèles { get; set; }
        public GestionnaireDeScènes GestionScènes { get; set; }
        public Random GénérateurAléatoire { get; set; }
        //public Terrain Map { get; set; }
        //public int[,] Minimap { get; set; }
        //public UI InterfaceUtilisateur { get; set; }

        //public Héros BaldorLeBrave { get; set; }
        //public Monstre faggit { get; set; }

        public RPG()
        {
            PériphériqueGraphique = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            PériphériqueGraphique.SynchronizeWithVerticalRetrace = false;
            PériphériqueGraphique.PreferredBackBufferHeight = 720;
            PériphériqueGraphique.PreferredBackBufferWidth = 1280;
            //PériphériqueGraphique.IsFullScreen = true;
            IsFixedTimeStep = false;
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            Vector3 positionCaméra = new Vector3(0, 10, 0);
            Vector3 cibleCaméra = new Vector3(0, 9, -10);

            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeModèles = new RessourcesManager<Model>(this, "Models");
            GénérateurAléatoire = new Random();
            GestionInput = new InputManager(this);
            GestionSprites = new SpriteBatch(GraphicsDevice);

            //Map = new Terrain(this, Vector3.Zero, new Vector2(300, 300), GestionnaireDeTextures.Find("Sand"), GestionnaireDeTextures.Find("heightmap"));
            //BaldorLeBrave = new Héros(this, "unicorn", 10f, new Vector3(1, 0, -5), new Vector3(0, 0, 0), 0.01f, "Charlie The Hurrnicorn", 30f, MathHelper.Pi * 4, false, 42, 15, 5, 8, 6, 0.8f);
            //faggit = new Monstre(this, BaldorLeBrave, "unicorn", 8f, new Vector3(2, 0, -9), new Vector3(0, 0, 0), 0.01f, "faggit", 25f, MathHelper.Pi * 4, false, 21, 3, 6, 4, 1f, 15, 50);
            //InterfaceUtilisateur = new UI(this, BaldorLeBrave, GestionnaireDeTextures.Find("UI"), GestionnaireDeTextures.Find("Vie"), GestionnaireDeTextures.Find("Mana"));
            //CaméraJeu = new CaméraThirdPerson(this, GestionInput, positionCaméra, BaldorLeBrave, Vector3.Up, 0.01f);

            // Services
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeModèles);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
            Services.AddService(typeof(Random), GénérateurAléatoire);


            Components.Add(GestionInput);

            GestionScènes = new GestionnaireDeScènes(this, GestionSprites, GestionnaireDeFonts, GestionnaireDeTextures, GestionnaireDeModèles, GestionInput);
            Components.Add(GestionScènes);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GestionInput.ÉtatSouris.RightButton == ButtonState.Pressed)
            //{
            //   Vector3 positionCible = GestionInput.GetPositionSouris3d();
            //   Vector2 positionCoordCible = new Vector2(positionCible.X, positionCible.Z);
            //   if ((positionCoordCible.X != 0 || positionCoordCible.Y != 0) && (positionCoordCible.X != Hurrnicorn.PositionCoord.X || positionCoordCible.Y != Hurrnicorn.PositionCoord.Y))
            //   {
            //      Hurrnicorn.DébuterDéplacement(positionCoordCible);
            //   }
            //   //Hurrnicorn.DébuterRotation(MathHelper.PiOver2);
            //}

            // Update Audio Engine
            Soundtrack.Update();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            base.Draw(gameTime);
        }

        protected override void LoadContent()
        {
            LoadCursor();
            base.LoadContent();
        }

        private void LoadCursor()
        {
            System.Windows.Forms.Cursor myCursor = NativeMethods.LoadCustomCursor(@"Content\Cursors\D2_hand.ani");
            System.Windows.Forms.Form winForm = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this.Window.Handle);
            winForm.Cursor = myCursor;
        }
    }

    // Service de changement de curseur
    static class NativeMethods
    {
        public static System.Windows.Forms.Cursor LoadCustomCursor(string path)
        {
            IntPtr hCurs = LoadCursorFromFile(path);
            if (hCurs == IntPtr.Zero) throw new Win32Exception();
            var curs = new System.Windows.Forms.Cursor(hCurs);
            // Note: force the cursor to own the handle so it gets released properly
            var fi = typeof(System.Windows.Forms.Cursor).GetField("ownHandle", BindingFlags.NonPublic | BindingFlags.Instance);
            fi.SetValue(curs, true);
            return curs;
        }
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern IntPtr LoadCursorFromFile(string path);
    }
}
