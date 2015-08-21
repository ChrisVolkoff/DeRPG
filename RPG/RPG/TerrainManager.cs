using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace AtelierXNA
{
    public enum Zones { Gazon, Neige, Désert, Lave1, Lave2 }

    public class TerrainManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        RPG Jeu { get; set; }
        ScèneDeJeu ScèneJeu { get; set; }
        const float DIAGONALE_UNITAIRE = 1.41421354f;
        const float ÉCHELLE_HAUTEUR = 0.25f;
        public float[,] HeightMap { get; private set; }
        string NomHeightMap { get; set; }

        public float HauteurMax { get; private set; }
        public float HauteurMin { get; private set; }
        public Terrain[] ListeTerrains { get; private set; }
        public int NbZones { get; private set; }
        public Vector2 Étendue { get; private set; }
        public FogData[] Fogs { get; set; }
        string[] NomsTextures { get; set; }
        public List<string> ListNomMusic { get; set; }

        Zones ZoneActuelle_;
        public Zones ZoneActuelle
        {
            get { return ZoneActuelle_; }
            set
            {
                if (value != ZoneActuelle_ && !ScèneJeu.FinalBoss.HasAggroed && !((value == Zones.Lave1 && ZoneActuelle_ == Zones.Lave2) || (value == Zones.Lave2 && ZoneActuelle_ == Zones.Lave1)))
                {
                    Soundtrack.StartSongCue(ListeTerrains[(int)value].NomMusicSpécifique);
                }
                ZoneActuelle_ = value;
            }
        }

        public TerrainManager(RPG jeu, ScèneDeJeu scèneJeu, int nbZones, Vector2 étendue, string[] nomstextures, FogData[] fogs, Texture2D heightMap, List<string> listNomMusic)
            : base(jeu)
        {
            Jeu = jeu;
            ScèneJeu = scèneJeu;
            NbZones = nbZones;
            Étendue = étendue;
            Fogs = fogs;
            ListNomMusic = listNomMusic;
            NomsTextures = nomstextures;
            Color[] couleursHeightMap = new Color[heightMap.Width * heightMap.Height];
            heightMap.GetData(couleursHeightMap);
            HeightMap = new float[heightMap.Width, heightMap.Height];
            HauteurMax = 0;
            HauteurMin = 256 * ÉCHELLE_HAUTEUR;

            for (int i = 0; i < heightMap.Width; ++i)
            {
                for (int j = 0; j < heightMap.Height; ++j)
                {
                    HeightMap[i, heightMap.Height - 1 - j] = couleursHeightMap[i + j * heightMap.Width].R * ÉCHELLE_HAUTEUR;
                    HauteurMax = Math.Max(HeightMap[i, heightMap.Height - 1 - j], HauteurMax);
                    HauteurMin = Math.Min(HeightMap[i, heightMap.Height - 1 - j], HauteurMin);
                }
            }
            ListeTerrains = new Terrain[NbZones];
            for (int i = 0; i < NbZones; ++i)
            {
                ListeTerrains[i] = new Terrain(Jeu, ScèneJeu, new Vector3(0, 0, (-1) * i * Étendue.Y), Étendue, new Vector2(heightMap.Width - 1, (heightMap.Height - 1) / NbZones), NomsTextures[i], Fogs[i], HeightMap, i * ((heightMap.Height) / NbZones), ListNomMusic[i]);
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
            Soundtrack.StartSongCue(ListeTerrains[(int)ZoneActuelle_].NomMusicSpécifique);
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
            ZoneActuelle = GetZoneFromZ(ScèneJeu.BaldorLeBrave.PositionCoord.Y);
        }

        private Zones GetZoneFromZ(float Z)
        {
            return (Zones)((int)Z / (-1 * ScèneDeJeu.ÉTENDUE_MAP));
        }

        public float CalculerHauteurPoint(Vector2 point)
        {
            float hauteur = 0;
            Vector2 index = new Vector2((point.X) * (HeightMap.GetLength(0) - 1) / Étendue.X, -1 * (point.Y) * (HeightMap.GetLength(1) - 1) / (Étendue.Y * NbZones));
            int borneInférieureX = (int)Math.Floor(index.X);
            int borneSupérieureX = (int)Math.Ceiling(index.X + 0.00001f);
            int borneInférieureY = (int)Math.Floor(index.Y);
            int borneSupérieureY = (int)Math.Ceiling(index.Y + 0.00001f);
            Vector2 pointHautGauche = new Vector2(borneInférieureX, borneSupérieureY);
            Vector2 pointHautDroite = new Vector2(borneSupérieureX, borneSupérieureY);
            Vector2 pointBasGauche = new Vector2(borneInférieureX, borneInférieureY);
            Vector2 pointBasDroite = new Vector2(borneSupérieureX, borneInférieureY);
            int borneXAGarder;
            int borneYAGarder;
            if (Vector2.Distance(pointHautDroite, index) > Vector2.Distance(pointBasGauche, index))
            {
                borneXAGarder = borneInférieureX;
                borneYAGarder = borneInférieureY;
            }
            else
            {
                borneXAGarder = borneSupérieureX;
                borneYAGarder = borneSupérieureY;
            }
            try
            {
                Vector2 vecteurDirecteur = pointHautGauche - pointBasDroite;
                Vector2 pointLePlusPres = pointBasDroite + (Vector2.Dot((index - pointBasDroite), vecteurDirecteur) / Vector2.Dot(vecteurDirecteur, vecteurDirecteur)) * vecteurDirecteur;
                float painis = Vector2.Distance(pointHautGauche, pointLePlusPres);
                float pootis = Vector2.Distance(pointBasDroite, pointLePlusPres);
                float poop = Vector2.Distance(pointHautGauche, pointLePlusPres) / (Vector2.Distance(pointBasDroite, pointLePlusPres) + Vector2.Distance(pointHautGauche, pointLePlusPres));
                float hauteurppp = MathHelper.Lerp(HeightMap[borneInférieureX, borneSupérieureY], HeightMap[borneSupérieureX, borneInférieureY], Vector2.Distance(pointHautGauche, pointLePlusPres) / DIAGONALE_UNITAIRE);
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
            int zoneCible = Jeu.GénérateurAléatoire.Next(0, NbZones);
            Vector2 randCoord = ListeTerrains[zoneCible].GetRandomCoordinate();
            return randCoord;
        }
    }
}
