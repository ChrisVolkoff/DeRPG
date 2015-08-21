using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace AtelierXNA
{
    public class ScèneDeJeu : Scène
    {
        public const int RÉSOLUTION_PAR_DÉFAUT_X = 1440, //la résolution que les positions constantes assument
                         RÉSOLUTION_PAR_DÉFAUT_Y = 900;

        const float PRÉCISION_VÉRIFICATION = 0.0005f;

        public const int ÉTENDUE_MAP = 600;
        const int NB_ZONES = 5;
        const int ESPACEMENT_ARBRES = 12;

        public const float ÉCHELLE_BOX_WURM = 0.0000000000000000000000000000000000002f;

        float TempsDepuisMAJ { get; set; }

        public Vector2 Scale { get; set; }

        // Éléments
        public HUD HeadsUpDisplay { get; set; }

        public Héros BaldorLeBrave { get; set; }
        public Monstre faggit { get; set; }

        public List<string[]> TexturesSkybox { get; private set; }
        public Skybox Box { get; set; }
        public Caméra CaméraJeu { get; set; }
        public InputManager3D GestionInput3D { get; private set; }
        public ProjectileManager ProjManager { get; private set; }
        public MonsterManager MonstManager { get; private set; }
        public CollisionManager CollisionManager { get; private set; }
        public HealthBarManager BarresManager { get; private set; }
        public TerrainManager MapManager { get; private set; }
        public NovaManager NovManager { get; private set; }
        public DoodadManager DoodManager { get; private set; }

        public Boss FinalBoss { get; private set; }

        public ScèneDeJeu(RPG jeu, GestionnaireDeScènes scènesMgr)
            : base(jeu, scènesMgr) { }

        // Add() components
        public override void Initialize()
        {
            Scale = new Vector2((float)Jeu.Window.ClientBounds.Width / RÉSOLUTION_PAR_DÉFAUT_X, (float)Jeu.Window.ClientBounds.Height / RÉSOLUTION_PAR_DÉFAUT_Y);
            Vector3 positionCaméra = new Vector3(300, 70, -10);
            TexturesSkybox = new List<string[]>();
            string[] TexturesTerrain = new string[] {"Terrain\\Grass",
                                              "Terrain\\Snow",
                                              "Terrain\\Sand",
                                              "Terrain\\Lava",
                                              "Terrain\\Lava"};
           FogData[] fogData = new FogData[] {new FogData(new Vector3(0.7f, 1f, 0.8f), 30, 450),
                                              new FogData(new Vector3(0.3f, 0.3f, 0.35f), 30, 450),
                                              new FogData(new Vector3(1f, 0.8f, 0.1f), -200, 1500),
                                              new FogData(new Vector3(1f, 0.6f, 0.1f), 20, 450),
                                              new FogData(new Vector3(0.70f, 0.32f, 0.0f), 10, 420)};
            
            List<string> listNomMusic = new List<string>() { "m_act1", "m_act2", "m_act3", "m_act4", "m_act4" };


            TexturesSkybox.Add(new string[]{"DarkBox\\topdark", 
                                              "DarkBox\\frontdark",
                                               "DarkBox\\backdark",
                                               "DarkBox\\leftdark",
                                               "DarkBox\\rightdark",
                                               "DarkBox\\botdark"} );

            string[] SpellIcons = new string[] {"HUD\\Spells\\ChaosBoltButton",
                                                      "HUD\\Spells\\PlasmaFieldButton",
                                                      "HUD\\Spells\\SlowTimeButton",
                                                      "HUD\\Spells\\SpellstealButton"};

            string[] SpellIconsAlt = new string[] {null,
                                                         null,
                                                         "HUD\\Spells\\TimeLockButton",
                                                         null};

            string[] SpellTooltips = new string[] {"Content\\Text\\Firebolt.txt",
                                                   "Content\\Text\\Nova.txt",
                                                   "Content\\Text\\SlowMissiles.txt",
                                                   "Content\\Text\\MindControl.txt"};

            string[] SpellTooltipsAlt = new string[] {null,
                                                      null,
                                                      "Content\\Text\\SlowMissilesAlt.txt",
                                                      null};

            //Vector3 positionCaméra = new Vector3(0, 10, 0);

            GestionInput3D = new InputManager3D(Jeu, this);
            MapManager = new TerrainManager(Jeu, this, NB_ZONES, new Vector2(ÉTENDUE_MAP, ÉTENDUE_MAP), TexturesTerrain, fogData, GestionnaireDeTextures.Find("Terrain\\heightmap3.0"), listNomMusic);
            BaldorLeBrave = new Héros(Jeu, this, "dude", 0.2f, 1f,                         
                                       new Vector3(300, 0, -50), new Vector3(0, MathHelper.PiOver2, 0), new Vector3(0, -MathHelper.PiOver2, 0),    
                                       "Baldor Le Brave", 
                                       35f, MathHelper.Pi * 4, true,
                                       42, 18, 0, 5, 3, 0.8f,
                                       "nova");
            CaméraJeu = new CaméraThirdPerson(Jeu, GestionInput3D, positionCaméra, BaldorLeBrave, Vector3.Up);
            Box = new Skybox(Jeu, this, "skybox2", TexturesSkybox[0], new Vector3(0,-1,0));
            FinalBoss = new Boss(Jeu, this, BaldorLeBrave, "Cyclops\\terrorwurm", 0.2f, ÉCHELLE_BOX_WURM, new Vector3(ÉTENDUE_MAP/2, 0, -ÉTENDUE_MAP*4.6f), new Vector3(0, 0, 0), new Vector3(0, -MathHelper.PiOver2, 0), "Demon Grostesque", 30f, MathHelper.Pi * 4, false, 400, 3, 6, 4, 1f, false, 15, 70, 42, 0);

            HeadsUpDisplay = new HUD(Jeu, this, BaldorLeBrave, "HUD\\HUD", "HUD\\Vie", "HUD\\Mana", "HUD\\Exp", 2.5f, SpellIcons, SpellIconsAlt, "HighTowerText", "HUD\\TextBackground", SpellTooltips, SpellTooltipsAlt);
            ProjManager = new ProjectileManager(Jeu, this, "OrangeBall", 0.01f);
            MonstManager = new MonsterManager(Jeu, this);
            CollisionManager = new CollisionManager(Jeu, this);
            BarresManager = new HealthBarManager(Jeu, this, "HUD\\healthbar", "HUD\\healthbarbg", "Stroke");
            NovManager = new NovaManager(Jeu, this);
            DoodManager = new DoodadManager (Jeu, this);
            

            ListeDesÉléments.Add(GestionInput3D);
            ListeDesÉléments.Add(new Afficheur3D(Jeu));

            ListeDesÉléments.Add(CaméraJeu);
            ListeDesÉléments.Add(Box);
            ListeDesÉléments.Add(MapManager);

            ListeDesÉléments.Add(BaldorLeBrave);
            ListeDesÉléments.Add(MonstManager);
            
            ListeDesÉléments.Add(FinalBoss);
            ListeDesÉléments.Add(ProjManager);
            ListeDesÉléments.Add(NovManager);
            ListeDesÉléments.Add(DoodManager);
            ListeDesÉléments.Add(CollisionManager);
            ListeDesÉléments.Add(HeadsUpDisplay);
            //ListeDesÉléments.Add(ExpBar);
            ListeDesÉléments.Add(BarresManager);
            ListeDesÉléments.Add(new AfficheurFPS(Jeu, "Arial"));

            

            base.Initialize();


            CréerDoodads();
            CréerMonstres();

        }

        private void CréerMonstres()
        {
           int[] nbmonstres = new int[] { 7, 9, 11, 12, 5 };
           for (int i = 0; i < NB_ZONES; ++i)
           {
              for (int j = 0; j < nbmonstres[i]; ++j)
              {
                 Vector2 position = MapManager.ListeTerrains[i].GetRandomCoordinate();
                 bool isrange = GénérerBoolAvecChance();
                 float range = isrange ? (float)(GénérateurAléatoire.NextDouble()+1)*20f*(i+1) : 15f;
                 MonstManager.CréerMonstre(BaldorLeBrave, "Cyclops\\terrorwurm", 0.1f, ÉCHELLE_BOX_WURM, new Vector3(position.X, 0, position.Y), Vector3.Zero, new Vector3(0, -MathHelper.PiOver2, 0),
                    "Ver Grotesque", GénérateurAléatoire.Next(15, 15 + 10 * (i + 1)), MathHelper.Pi * 4, true, GénérateurAléatoire.Next(16, 16 + 14 * (i + 1)), GénérateurAléatoire.Next(0, 2 + 2 * i), (int)((GénérateurAléatoire.Next(3, 2 + (i+1) * 3))*(isrange?1.5f:1)),
                    GénérateurAléatoire.Next(1, 1 + i * 2), (float)(2 - (GénérateurAléatoire.NextDouble() * (float)(i) / 3f)), isrange, range, (float)(GénérateurAléatoire.NextDouble()+1) * (i + 1) * 15f + 20f, GénérateurAléatoire.Next(i + 1, 2 * (i + 1)), MonstManager.GetID()); 
              }
           }
        }

        private bool GénérerBoolAvecChance()
        {
           bool retour = GénérateurAléatoire.Next(0, 2) == 1;
           return retour;
        }

        private void CréerDoodads()
        {
           for (int i = 1; i < ÉTENDUE_MAP / ESPACEMENT_ARBRES; ++i)
           {
              DoodManager.CréerDoodad("LordaeronTree\\LordaeronTree0", "LordaeronTree\\LordaeronSummerTree", 1.2f, 1f, new Vector2(i * ESPACEMENT_ARBRES, -ESPACEMENT_ARBRES), new Vector3(0, MathHelper.PiOver2, 0), false);
              //ListeDesÉléments.Add(new Doodad(Jeu, this, "LordaeronTree\\LordaeronTree0", GestionnaireDeTextures.Find("LordaeronTree\\LordaeronWinterTree"), 1f, new Vector3(i * 15, 0, -10), new Vector3(0, 0, 0)));
           }

           for (int i = 2 + (ÉTENDUE_MAP * 0) / ESPACEMENT_ARBRES; i <= 2 + (ÉTENDUE_MAP * 1) / ESPACEMENT_ARBRES; ++i)
           {
              DoodManager.CréerDoodad("LordaeronTree\\LordaeronTree0", "LordaeronTree\\LordaeronWinterTree", 1.2f, 1f, new Vector2(ESPACEMENT_ARBRES, -ESPACEMENT_ARBRES * i), new Vector3(0, MathHelper.PiOver2, 0), false);
              DoodManager.CréerDoodad("LordaeronTree\\LordaeronTree0", "LordaeronTree\\LordaeronWinterTree", 1.2f, 1f, new Vector2(ÉTENDUE_MAP - ESPACEMENT_ARBRES, -ESPACEMENT_ARBRES * i), new Vector3(0, MathHelper.PiOver2, 0), false);

              //ListeDesÉléments.Add(new Doodad(Jeu, this, "LordaeronTree\\LordaeronTree0", GestionnaireDeTextures.Find("LordaeronTree\\LordaeronWinterTree"), 1f, new Vector3(i * 15, 0, -10), new Vector3(0, 0, 0)));
           }
           for (int i = 2 + (ÉTENDUE_MAP * 1) / ESPACEMENT_ARBRES; i <= (ÉTENDUE_MAP * 2) / ESPACEMENT_ARBRES; ++i)
           {
              DoodManager.CréerDoodad("LordaeronTree\\LordaeronTree0", "LordaeronTree\\LordaeronSnowTree", 1.2f, 1f, new Vector2(ESPACEMENT_ARBRES, -ESPACEMENT_ARBRES * i), new Vector3(0, MathHelper.PiOver2, 0), false);
              DoodManager.CréerDoodad("LordaeronTree\\LordaeronTree0", "LordaeronTree\\LordaeronSnowTree", 1.2f, 1f, new Vector2(ÉTENDUE_MAP - ESPACEMENT_ARBRES, -ESPACEMENT_ARBRES * i), new Vector3(0, MathHelper.PiOver2, 0), false);

              //ListeDesÉléments.Add(new Doodad(Jeu, this, "LordaeronTree\\LordaeronTree0", GestionnaireDeTextures.Find("LordaeronTree\\LordaeronWinterTree"), 1f, new Vector3(i * 15, 0, -10), new Vector3(0, 0, 0)));
           }
           for (int i = (ÉTENDUE_MAP * 2) / ESPACEMENT_ARBRES; i <= (ÉTENDUE_MAP * 3) / ESPACEMENT_ARBRES; ++i)
           {
              DoodManager.CréerDoodad("BarrenTree\\BarrenTree", "BarrenTree\\BarrensTree", 1.5f, 1f, new Vector2(ESPACEMENT_ARBRES, -ESPACEMENT_ARBRES * i), new Vector3(0, MathHelper.PiOver2, 0), true);
              DoodManager.CréerDoodad("BarrenTree\\BarrenTree", "BarrenTree\\BarrensTree", 1.5f, 1f, new Vector2(ÉTENDUE_MAP - ESPACEMENT_ARBRES, -ESPACEMENT_ARBRES * i), new Vector3(0, MathHelper.PiOver2, 0), true);

              //ListeDesÉléments.Add(new Doodad(Jeu, this, "LordaeronTree\\LordaeronTree0", GestionnaireDeTextures.Find("LordaeronTree\\LordaeronWinterTree"), 1f, new Vector3(i * 15, 0, -10), new Vector3(0, 0, 0)));
           }
           for (int i = (ÉTENDUE_MAP * 3) / ESPACEMENT_ARBRES; i <= ((ÉTENDUE_MAP * NB_ZONES) / ESPACEMENT_ARBRES) - 1; ++i)
           {
              DoodManager.CréerDoodad("BarrenTree\\BarrenTree", "BarrenTree\\BarrensTreeBlight", 1.5f, 1f, new Vector2(ESPACEMENT_ARBRES, -ESPACEMENT_ARBRES * i), new Vector3(0, MathHelper.PiOver2, 0), true);
              DoodManager.CréerDoodad("BarrenTree\\BarrenTree", "BarrenTree\\BarrensTreeBlight", 1.5f, 1f, new Vector2(ÉTENDUE_MAP - ESPACEMENT_ARBRES, -ESPACEMENT_ARBRES * i), new Vector3(0, MathHelper.PiOver2, 0), true);

              //ListeDesÉléments.Add(new Doodad(Jeu, this, "LordaeronTree\\LordaeronTree0", GestionnaireDeTextures.Find("LordaeronTree\\LordaeronWinterTree"), 1f, new Vector3(i * 15, 0, -10), new Vector3(0, 0, 0)));
           }
           for (int i = 1; i < ÉTENDUE_MAP / ESPACEMENT_ARBRES; ++i)
           {
              DoodManager.CréerDoodad("BarrenTree\\BarrenTree", "BarrenTree\\BarrensTreeBlight", 1.5f, 1f, new Vector2(i * ESPACEMENT_ARBRES, -(ÉTENDUE_MAP * 5 - ESPACEMENT_ARBRES)), new Vector3(0, MathHelper.PiOver2, 0), true);
              //ListeDesÉléments.Add(new Doodad(Jeu, this, "LordaeronTree\\LordaeronTree0", GestionnaireDeTextures.Find("LordaeronTree\\LordaeronWinterTree"), 1f, new Vector3(i * 15, 0, -10), new Vector3(0, 0, 0)));
           }
        }

        // Appelée lorsque la scène est activée (Initialize, mais à chaque activation de la scène)
        public override void Activate()
        {
           // GestionInput3D.Initialize();
            GestionInput.Enabled = false; //disable the InputManager to use only the InputManager3D
            MapManager.StartMusic();

            base.Activate();
        }

        public override void Update(GameTime gameTime)
        {
           TempsDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
           if (TempsDepuisMAJ >= Jeu.INTERVALLE_MAJ)
           {
              if (GestionInput3D.EstNouvelleTouche(Keys.Escape))
              {
                 SceneManager.ChangerDeScène(Scènes.MenuPrincipal);
              }


              TempsDepuisMAJ = 0;
           }
           base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
