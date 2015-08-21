using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AtelierXNA
{
    public enum Zones { Gazon, Neige, D�sert, Lave1, Lave2 }

    public class TerrainManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        RPG Jeu { get; set; }
        Sc�neDeJeu Sc�neJeu { get; set; }
        const float DIAGONALE_UNITAIRE = 1.41421354f;
        const float �CHELLE_HAUTEUR = 0.25f;
        public float[,] HeightMap { get; private set; }
        string NomHeightMap { get; set; }

        public float HauteurMax { get; private set; }
        public float HauteurMin { get; private set; }
        public Terrain[] ListeTerrains { get; private set; }
        public int NbZones { get; private set; }
        public Vector2 �tendue { get; private set; }
        public FogData[] Fogs { get; set; }
        string[] NomsTextures { get; set; }
        public List<string> ListNomMusic { get; set; }

        Zones ZoneActuelle_;
        public Zones ZoneActuelle
        {
            get { return ZoneActuelle_; }
            set
            {
                if (value != ZoneActuelle_ && !Sc�neJeu.FinalBoss.HasAggroed && !((value == Zones.Lave1 && ZoneActuelle_ == Zones.Lave2) || (value == Zones.Lave2 && ZoneActuelle_ == Zones.Lave1)))
                {
                    Soundtrack.StartSongCue(ListeTerrains[(int)value].NomMusicSp�cifique);
                }
                ZoneActuelle_ = value;
            }
        }

        public TerrainManager(RPG jeu, Sc�neDeJeu sc�neJeu, int nbZones, Vector2 �tendue, string[] nomstextures, FogData[] fogs, Texture2D heightMap, List<string> listNomMusic)
            : base(jeu)
        {
            Jeu = jeu;
            Sc�neJeu = sc�neJeu;
            NbZones = nbZones;
            �tendue = �tendue;
            Fogs = fogs;
            ListNomMusic = listNomMusic;
            NomsTextures = nomstextures;
            Color[] couleursHeightMap = new Color[heightMap.Width * heightMap.Height];
            heightMap.GetData(couleursHeightMap);
            HeightMap = new float[heightMap.Width, heightMap.Height];
            HauteurMax = 0;
            HauteurMin = 256 * �CHELLE_HAUTEUR;

            for (int i = 0; i < heightMap.Width; ++i)
            {
                for (int j = 0; j < heightMap.Height; ++j)
                {
                    HeightMap[i, heightMap.Height - 1 - j] = couleursHeightMap[i + j * heightMap.Width].R * �CHELLE_HAUTEUR;
                    HauteurMax = Math.Max(HeightMap[i, heightMap.Height - 1 - j], HauteurMax);
                    HauteurMin = Math.Min(HeightMap[i, heightMap.Height - 1 - j], HauteurMin);
                }
            }
            ListeTerrains = new Terrain[NbZones];
            for (int i = 0; i < NbZones; ++i)
            {
                ListeTerrains[i] = new Terrain(Jeu, Sc�neJeu, new Vector3(0, 0, (-1) * i * �tendue.Y), �tendue, new Vector2(heightMap.Width - 1, (heightMap.Height - 1) / NbZones), NomsTextures[i], Fogs[i], HeightMap, i * ((heightMap.Height) / NbZones), ListNomMusic[i]);
                ListeTerrains[i].Initialize();
            }
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public void StartMusic()
        {
            Soundtrack.StartSongCue(ListeTerrains[(int)ZoneActuelle_].NomMusicSp�cifique);
        }

        public override void Update(GameTime gameTime)
        {
            UpdatePosition();
            foreach (Terrain zone in ListeTerrains)
            {
                zone.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (Terrain zone in ListeTerrains)
            {
                zone.Draw(gameTime);
            }

            base.Update(gameTime);
        }

        private void UpdatePosition()
        {
            ZoneActuelle = GetZoneFromZ(Sc�neJeu.BaldorLeBrave.PositionCoord.Y);
        }

        private Zones GetZoneFromZ(float Z)
        {
            return (Zones)((int)Z / (-1 * Sc�neDeJeu.�TENDUE_MAP));
        }

        public float CalculerHauteurPoint(Vector2 point)
        {
            float hauteur = 0;
            Vector2 index = new Vector2((point.X) * (HeightMap.GetLength(0) - 1) / �tendue.X, -1 * (point.Y) * (HeightMap.GetLength(1) - 1) / (�tendue.Y * NbZones));
            int borneInf�rieureX = (int)Math.Floor(index.X);
            int borneSup�rieureX = (int)Math.Ceiling(index.X + 0.00001f);
            int borneInf�rieureY = (int)Math.Floor(index.Y);
            int borneSup�rieureY = (int)Math.Ceiling(index.Y + 0.00001f);
            Vector2 pointHautGauche = new Vector2(borneInf�rieureX, borneSup�rieureY);
            Vector2 pointHautDroite = new Vector2(borneSup�rieureX, borneSup�rieureY);
            Vector2 pointBasGauche = new Vector2(borneInf�rieureX, borneInf�rieureY);
            Vector2 pointBasDroite = new Vector2(borneSup�rieureX, borneInf�rieureY);
            int borneXAGarder;
            int borneYAGarder;
            if (Vector2.Distance(pointHautDroite, index) > Vector2.Distance(pointBasGauche, index))
            {
                borneXAGarder = borneInf�rieureX;
                borneYAGarder = borneInf�rieureY;
            }
            else
            {
                borneXAGarder = borneSup�rieureX;
                borneYAGarder = borneSup�rieureY;
            }
            try
            {
                Vector2 vecteurDirecteur = pointHautGauche - pointBasDroite;
                Vector2 pointLePlusPres = pointBasDroite + (Vector2.Dot((index - pointBasDroite), vecteurDirecteur) / Vector2.Dot(vecteurDirecteur, vecteurDirecteur)) * vecteurDirecteur;
                float painis = Vector2.Distance(pointHautGauche, pointLePlusPres);
                float pootis = Vector2.Distance(pointBasDroite, pointLePlusPres);
                float poop = Vector2.Distance(pointHautGauche, pointLePlusPres) / (Vector2.Distance(pointBasDroite, pointLePlusPres) + Vector2.Distance(pointHautGauche, pointLePlusPres));
                float hauteurppp = MathHelper.Lerp(HeightMap[borneInf�rieureX, borneSup�rieureY], HeightMap[borneSup�rieureX, borneInf�rieureY], Vector2.Distance(pointHautGauche, pointLePlusPres) / DIAGONALE_UNITAIRE);
                hauteur = MathHelper.Lerp(hauteurppp, HeightMap[borneXAGarder, borneYAGarder], Vector2.Distance(pointLePlusPres, index) / (DIAGONALE_UNITAIRE / 2));
                float fuck = Vector2.Distance(pointLePlusPres, index) / (DIAGONALE_UNITAIRE / 2);
            }
            catch
            {
                hauteur = 0;
            }

            return hauteur;
        }

        public Vector2 GetRandomCoordinate()
        {
            int zoneCible = Jeu.G�n�rateurAl�atoire.Next(0, NbZones);
            Vector2 randCoord = ListeTerrains[zoneCible].GetRandomCoordinate();
            return randCoord;
        }
    }
}
