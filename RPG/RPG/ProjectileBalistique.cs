using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
    public class ProjectileBalistique : ObjetDeBasePhysique
    {
        const float ACCÉLÉRATION_GRAVITATIONNELLE = 9.81f;
        const float OFFSET_HAUTEUR_DÉPART = 10f;
        const float ÉCHELLE_TEMPS = 2f;

        public Combattant Propriétaire { get; private set; }
        public bool IsSlowing { get; set; }
        public int Damage { get; private set; }
        public Vector3 PositionInitiale { get; private set; }
        public float Vitesse { get; private set; }
        public float AngleTheta { get; private set; }
        public float AngleBeta { get; private set; }
        public float Temps { get; private set; }
        public float TempsÉcouléDepuisMAJ { get; private set; }

        public int ID { get; private set; }
        public bool ToRemove { get { return (Position.Y <= ScèneJeu.MapManager.CalculerHauteurPoint(new Vector2(Position.X, Position.Z))) || (Temps >= TempsFinal) || EstEnCollision; } }
        public bool EstEnCollision { get; set; }

        # region Informations trajectoire
        private float TempsInitialRéel { get; set; }
        private float TempsInitial { get; set; }
        private float TempsFinal { get; set; }
        private float TempsMax { get; set; }
        private Vector3 PositionMax { get; set; }
        private Vector3 PositionFinale { get; set; }
        private float Portée { get; set; }
        #endregion

        // Constructeur normal: position, vitesse, angle theta, angle beta
        public ProjectileBalistique(RPG jeu, ScèneDeJeu scèneJeu, Combattant propriétaire, int damage, int id, float vitesseInitiale, float angleTheta, float angleBeta, string nomModèle, float échelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
           : base(jeu, scèneJeu, nomModèle, échelleInitiale, 1f, rotationInitiale, new Vector3(0, 0, 0), positionInitiale)
        {
            Propriétaire = propriétaire;
            Damage = damage;
            ID = id;

            PositionInitiale = new Vector3(positionInitiale.X, positionInitiale.Y+OFFSET_HAUTEUR_DÉPART, positionInitiale.Z);
            Vitesse = vitesseInitiale;
            AngleTheta = MathHelper.ToRadians(angleTheta);
            AngleBeta = MathHelper.ToRadians(angleBeta);

        }

        // Constructeur: position initiale, position finale, portée max
        public ProjectileBalistique(RPG jeu, ScèneDeJeu scèneJeu, Combattant propriétaire, int damage, int id, Vector3 positionInitiale, Vector3 positionFinale, float portéeMax, string nomModèle, float échelleInitiale, Vector3 rotationInitiale)
           : base(jeu, scèneJeu, nomModèle, échelleInitiale, 1f, rotationInitiale, new Vector3(0, 0, 0), positionInitiale)
        {
            Propriétaire = propriétaire;
            Damage = damage;
            ID = id;

            PositionInitiale = new Vector3(positionInitiale.X, positionInitiale.Y+OFFSET_HAUTEUR_DÉPART, positionInitiale.Z);

            float portéeRéelle = (float)Math.Sqrt(((positionFinale.X - PositionInitiale.X) * (positionFinale.X - PositionInitiale.X)) + ((positionFinale.Z - PositionInitiale.Z) * (positionFinale.Z - PositionInitiale.Z)));
            float h = PositionInitiale.Y - positionFinale.Y;

            Vitesse = CalculerVitesseSelonPortéeMax(portéeMax, h, ACCÉLÉRATION_GRAVITATIONNELLE);
            AngleTheta = CalculerAngleThetaSelonVitesse(Vitesse, portéeRéelle, h, ACCÉLÉRATION_GRAVITATIONNELLE);
            AngleBeta = (float)Math.Atan2((positionFinale.X - positionInitiale.X), (positionFinale.Z - positionInitiale.Z));

        }

        public override void Initialize()
        {
            Temps = 0;
            TempsÉcouléDepuisMAJ = 0;

            IsSlowing = false;

            TempsInitialRéel = CalculerTempsInitialRéel(Vitesse, AngleTheta, ACCÉLÉRATION_GRAVITATIONNELLE, PositionInitiale.Y);
            TempsInitial = 0;
            TempsFinal = CalculerTempsFinal(Vitesse, AngleTheta, ACCÉLÉRATION_GRAVITATIONNELLE, PositionInitiale.Y);
            TempsMax = CalculerTempsMax(Vitesse, AngleTheta, ACCÉLÉRATION_GRAVITATIONNELLE, PositionInitiale.Y);
            PositionMax = new Vector3(GetX(TempsMax, PositionInitiale.X), GetY(TempsMax, PositionInitiale.Y), GetZ(TempsMax, PositionInitiale.Z));
            PositionFinale = new Vector3(GetX(TempsFinal, PositionInitiale.X), GetY(TempsFinal, PositionInitiale.Y), GetZ(TempsFinal, PositionInitiale.Z));
            Portée = CalculerPortée(Vitesse, AngleTheta, ACCÉLÉRATION_GRAVITATIONNELLE, PositionInitiale.Y);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            TempsÉcouléDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (TempsÉcouléDepuisMAJ >= Jeu.INTERVALLE_MAJ)
            {
                UpdatePosition();
                RecalculerMonde();
                if (!IsSlowing)
                {
                    Temps = MathHelper.Clamp(Temps + TempsÉcouléDepuisMAJ*ÉCHELLE_TEMPS, TempsInitial, TempsFinal);
                }
                else
                {
                   Temps = MathHelper.Clamp(Temps + (TempsÉcouléDepuisMAJ * Héros.SLOW_PROJ_SCALE * ÉCHELLE_TEMPS), TempsInitial, TempsFinal);
                }
                TempsÉcouléDepuisMAJ = 0;
                //Debugger();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        private void UpdatePosition()
        {
            Position = new Vector3(GetX(Temps, PositionInitiale.X), GetY(Temps, PositionInitiale.Y), GetZ(Temps, PositionInitiale.Z));
        }

        private float GetX(float t, float xi)
        {
            return (Vitesse * (float)Math.Cos((double)AngleTheta) * t * (float)Math.Sin((double)AngleBeta)) + xi;
        }

        private float GetZ(float t, float zi)
        {
            return (Vitesse * (float)Math.Cos((double)AngleTheta) * t * (float)Math.Cos((double)AngleBeta)) + zi;
        }

        private float GetY(float t, float yi)
        {
            return ((-(0.5f)) * ACCÉLÉRATION_GRAVITATIONNELLE * t * t) + (Vitesse * (float)Math.Sin((double)AngleTheta) * t) + yi;

        }

        private void RecalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        private float CalculerVitesseSelonPortéeMax(float portéeMax, float h, float g)
        {
            return (float)Math.Sqrt((portéeMax + h) * g) * portéeMax / (portéeMax + h);
        }

        private float CalculerAngleThetaSelonVitesse(float v, float portéeRéelle, float h, float g)
        {
            return (float)Math.Atan((-h * (v * v + g * h + (float)Math.Sqrt(v * v * v * v + 2 * v * v * g * h - portéeRéelle * portéeRéelle * g * g)) / (h * h + portéeRéelle * portéeRéelle) + g) / (v * (float)Math.Sqrt((v * v + g * h + (float)Math.Sqrt(v * v * v * v + 2 * v * v * g * h - portéeRéelle * portéeRéelle * g * g)) / (h * h + portéeRéelle * portéeRéelle))) * (v / ((float)Math.Sqrt((v*v + g * h + (float)Math.Sqrt(v * v * v * v + 2 * v * v * g * h - portéeRéelle * portéeRéelle * g * g)) / (h * h + portéeRéelle * portéeRéelle)) * portéeRéelle)));
        }

        private void Debugger()
        {
            Debug.Clear();
            Debug.WriteLine("Enabled = " + (this.Enabled).ToString() + "; Visible = " + (this.Visible).ToString());
            Debug.WriteLine("Temps initial réel:" + TempsInitialRéel.ToString());
            Debug.WriteLine("Temps initial:     " + TempsInitial.ToString());
            Debug.WriteLine("Temps final:       " + TempsFinal.ToString());
            Debug.WriteLine("Temps max:         " + TempsMax.ToString());
            Debug.WriteLine("Position initiale: " + PositionInitiale.ToString());
            Debug.WriteLine("Position finale:   " + PositionFinale.ToString());
            Debug.WriteLine("Position max:      " + PositionMax.ToString());
            Debug.WriteLine("Portée:            " + Portée.ToString());
            Debug.WriteLine("-------------------------------------------------");
            Debug.WriteLine("Position:          " + Position.ToString());
            Debug.WriteLine("Temps:             " + Temps.ToString());
            Debug.WriteLine("Vitesse initiale:  " + Vitesse.ToString());
            Debug.WriteLine("Angle theta:       " + AngleTheta.ToString());
            Debug.WriteLine("Angle beta:        " + AngleBeta.ToString());
        }

        // Informations sur la trajectoire
        private float CalculerTempsInitialRéel(float v, float theta, float a, float hauteurInitiale)
        {
            return (v * (float)Math.Sin(theta) - (float)Math.Sqrt(v * v * Math.Sin(theta) * Math.Sin(theta) + 2 * a * hauteurInitiale)) / a;
        }

        private float CalculerTempsInitial()
        {
            return 0;
        }

        private float CalculerTempsFinal(float v, float theta, float a, float hauteurInitiale)
        {
            return (v * (float)Math.Sin(theta) + (float)Math.Sqrt(v * v * Math.Sin(theta) * Math.Sin(theta) + 2 * a * hauteurInitiale)) / a;
        }

        private float CalculerTempsMax(float v, float theta, float a, float hauteurInitiale)
        {
            return ((CalculerTempsFinal(v, theta, a, hauteurInitiale) - CalculerTempsInitialRéel(v, theta, a, hauteurInitiale)) / 2) + CalculerTempsInitialRéel(v, theta, a, hauteurInitiale);
        }

        private float CalculerPortée(float v, float theta, float a, float hauteurInitiale)
        {
            return (v * v * (float)Math.Sin(theta) * (float)Math.Cos(theta) + v * (float)Math.Cos(theta) * (float)Math.Sqrt(v * v * Math.Sin(theta) * Math.Sin(theta) + 2 * a * hauteurInitiale)) / a;
        }

        public static float DistanceEntreDeuxPoints3D(Vector3 pt1, Vector3 pt2)
        {
            return (float)Math.Sqrt(Math.Pow((pt2.X - pt1.X), 2) + Math.Pow((pt2.Y - pt1.Y), 2) + Math.Pow((pt2.Z - pt1.Z), 2));
        }
    }
}
