using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using System.Windows.Forms; On ne peut l'utiliser � cause d'une confusion avec Keys d'XNA
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Reflection;

namespace AtelierXNA
{
    public class RPG : Microsoft.Xna.Framework.Game
    {
       public float INTERVALLE_MAJ = 0.01f;

        public GraphicsDeviceManager P�riph�riqueGraphique { get; private set; }
        public SpriteBatch GestionSprites { get; private set; }
        public InputManager GestionInput { get; private set; }
        public RessourcesManager<SpriteFont> GestionnaireDeFonts { get; set; }
        public RessourcesManager<Texture2D> GestionnaireDeTextures { get; set; }
        public RessourcesManager<Model> GestionnaireDeMod�les { get; set; }
        public GestionnaireDeSc�nes GestionSc�nes { get; set; }
        public Random G�n�rateurAl�atoire { get; set; }
        //public Terrain Map { get; set; }
        //public int[,] Minimap { get; set; }
        //public UI InterfaceUtilisateur { get; set; }

        //public H�ros BaldorLeBrave { get; set; }
        //public Monstre faggit { get; set; }

        public RPG()
        {
            P�riph�riqueGraphique = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            P�riph�riqueGraphique.SynchronizeWithVerticalRetrace = false;
            P�riph�riqueGraphique.PreferredBackBufferHeight = 720;
            P�riph�riqueGraphique.PreferredBackBufferWidth = 1280;
            //P�riph�riqueGraphique.IsFullScreen = true;
            IsFixedTimeStep = false;
            IsMouseVisible = true;

        }

        protected override void Initialize()
        {
            Vector3 positionCam�ra = new Vector3(0, 10, 0);
            Vector3 cibleCam�ra = new Vector3(0, 9, -10);

            GestionnaireDeFonts = new RessourcesManager<SpriteFont>(this, "Fonts");
            GestionnaireDeTextures = new RessourcesManager<Texture2D>(this, "Textures");
            GestionnaireDeMod�les = new RessourcesManager<Model>(this, "Models");
            G�n�rateurAl�atoire = new Random();
            GestionInput = new InputManager(this);
            GestionSprites = new SpriteBatch(GraphicsDevice);

            //Map = new Terrain(this, Vector3.Zero, new Vector2(300, 300), GestionnaireDeTextures.Find("Sand"), GestionnaireDeTextures.Find("heightmap"));
            //BaldorLeBrave = new H�ros(this, "unicorn", 10f, new Vector3(1, 0, -5), new Vector3(0, 0, 0), 0.01f, "Charlie The Hurrnicorn", 30f, MathHelper.Pi * 4, false, 42, 15, 5, 8, 6, 0.8f);
            //faggit = new Monstre(this, BaldorLeBrave, "unicorn", 8f, new Vector3(2, 0, -9), new Vector3(0, 0, 0), 0.01f, "faggit", 25f, MathHelper.Pi * 4, false, 21, 3, 6, 4, 1f, 15, 50);
            //InterfaceUtilisateur = new UI(this, BaldorLeBrave, GestionnaireDeTextures.Find("UI"), GestionnaireDeTextures.Find("Vie"), GestionnaireDeTextures.Find("Mana"));
            //Cam�raJeu = new Cam�raThirdPerson(this, GestionInput, positionCam�ra, BaldorLeBrave, Vector3.Up, 0.01f);

            // Services
            Services.AddService(typeof(InputManager), GestionInput);
            Services.AddService(typeof(RessourcesManager<SpriteFont>), GestionnaireDeFonts);
            Services.AddService(typeof(RessourcesManager<Texture2D>), GestionnaireDeTextures);
            Services.AddService(typeof(RessourcesManager<Model>), GestionnaireDeMod�les);
            Services.AddService(typeof(SpriteBatch), GestionSprites);
            Services.AddService(typeof(Random), G�n�rateurAl�atoire);


            Components.Add(GestionInput);

            GestionSc�nes = new GestionnaireDeSc�nes(this, GestionSprites, GestionnaireDeFonts, GestionnaireDeTextures, GestionnaireDeMod�les, GestionInput);
            Components.Add(GestionSc�nes);

            base.Initialize();
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GestionInput.�tatSouris.RightButton == ButtonState.Pressed)
            //{
            //   Vector3 positionCible = GestionInput.GetPositionSouris3d();
            //   Vector2 positionCoordCible = new Vector2(positionCible.X, positionCible.Z);
            //   if ((positionCoordCible.X != 0 || positionCoordCible.Y != 0) && (positionCoordCible.X != Hurrnicorn.PositionCoord.X || positionCoordCible.Y != Hurrnicorn.PositionCoord.Y))
            //   {
            //      Hurrnicorn.D�buterD�placement(positionCoordCible);
            //   }
            //   //Hurrnicorn.D�buterRotation(MathHelper.PiOver2);
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
