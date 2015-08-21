using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
    public class ProjectileBalistique : ObjetDeBasePhysique
    {
        const float ACC�L�RATION_GRAVITATIONNELLE = 9.81f;
        const float OFFSET_HAUTEUR_D�PART = 10f;
        const float �CHELLE_TEMPS = 2f;

        public Combattant Propri�taire { get; private set; }
        public bool IsSlowing { get; set; }
        public int Damage { get; private set; }
        public Vector3 PositionInitiale { get; private set; }
        public float Vitesse { get; private set; }
        public float AngleTheta { get; private set; }
        public float AngleBeta { get; private set; }
        public float Temps { get; private set; }
        public float Temps�coul�DepuisMAJ { get; private set; }

        public int ID { get; private set; }
        public bool ToRemove { get { return (Position.Y <= Sc�neJeu.MapManager.CalculerHauteurPoint(new Vector2(Position.X, Position.Z))) || (Temps >= TempsFinal) || EstEnCollision; } }
        public bool EstEnCollision { get; set; }

        # region Informations trajectoire
        private float TempsInitialR�el { get; set; }
        private float TempsInitial { get; set; }
        private float TempsFinal { get; set; }
        private float TempsMax { get; set; }
        private Vector3 PositionMax { get; set; }
        private Vector3 PositionFinale { get; set; }
        private float Port�e { get; set; }
        #endregion

        // Constructeur normal: position, vitesse, angle theta, angle beta
        public ProjectileBalistique(RPG jeu, Sc�neDeJeu sc�neJeu, Combattant propri�taire, int damage, int id, float vitesseInitiale, float angleTheta, float angleBeta, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale, Vector3 positionInitiale)
           : base(jeu, sc�neJeu, nomMod�le, �chelleInitiale, 1f, rotationInitiale, new Vector3(0, 0, 0), positionInitiale)
        {
            Propri�taire = propri�taire;
            Damage = damage;
            ID = id;

            PositionInitiale = new Vector3(positionInitiale.X, positionInitiale.Y+OFFSET_HAUTEUR_D�PART, positionInitiale.Z);
            Vitesse = vitesseInitiale;
            AngleTheta = MathHelper.ToRadians(angleTheta);
            AngleBeta = MathHelper.ToRadians(angleBeta);

        }

        // Constructeur: position initiale, position finale, port�e max
        public ProjectileBalistique(RPG jeu, Sc�neDeJeu sc�neJeu, Combattant propri�taire, int damage, int id, Vector3 positionInitiale, Vector3 positionFinale, float port�eMax, string nomMod�le, float �chelleInitiale, Vector3 rotationInitiale)
           : base(jeu, sc�neJeu, nomMod�le, �chelleInitiale, 1f, rotationInitiale, new Vector3(0, 0, 0), positionInitiale)
        {
            Propri�taire = propri�taire;
            Damage = damage;
            ID = id;

            PositionInitiale = new Vector3(positionInitiale.X, positionInitiale.Y+OFFSET_HAUTEUR_D�PART, positionInitiale.Z);

            float port�eR�elle = (float)Math.Sqrt(((positionFinale.X - PositionInitiale.X) * (positionFinale.X - PositionInitiale.X)) + ((positionFinale.Z - PositionInitiale.Z) * (positionFinale.Z - PositionInitiale.Z)));
            float h = PositionInitiale.Y - positionFinale.Y;

            Vitesse = CalculerVitesseSelonPort�eMax(port�eMax, h, ACC�L�RATION_GRAVITATIONNELLE);
            AngleTheta = CalculerAngleThetaSelonVitesse(Vitesse, port�eR�elle, h, ACC�L�RATION_GRAVITATIONNELLE);
            AngleBeta = (float)Math.Atan2((positionFinale.X - positionInitiale.X), (positionFinale.Z - positionInitiale.Z));

        }

        public override void Initialize()
        {
            Temps = 0;
            Temps�coul�DepuisMAJ = 0;

            IsSlowing = false;

            TempsInitialR�el = CalculerTempsInitialR�el(Vitesse, AngleTheta, ACC�L�RATION_GRAVITATIONNELLE, PositionInitiale.Y);
            TempsInitial = 0;
            TempsFinal = CalculerTempsFinal(Vitesse, AngleTheta, ACC�L�RATION_GRAVITATIONNELLE, PositionInitiale.Y);
            TempsMax = CalculerTempsMax(Vitesse, AngleTheta, ACC�L�RATION_GRAVITATIONNELLE, PositionInitiale.Y);
            PositionMax = new Vector3(GetX(TempsMax, PositionInitiale.X), GetY(TempsMax, PositionInitiale.Y), GetZ(TempsMax, PositionInitiale.Z));
            PositionFinale = new Vector3(GetX(TempsFinal, PositionInitiale.X), GetY(TempsFinal, PositionInitiale.Y), GetZ(TempsFinal, PositionInitiale.Z));
            Port�e = CalculerPort�e(Vitesse, AngleTheta, ACC�L�RATION_GRAVITATIONNELLE, PositionInitiale.Y);
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            Temps�coul�DepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (Temps�coul�DepuisMAJ >= Jeu.INTERVALLE_MAJ)
            {
                UpdatePosition();
                RecalculerMonde();
                if (!IsSlowing)
                {
                    Temps = MathHelper.Clamp(Temps + Temps�coul�DepuisMAJ*�CHELLE_TEMPS, TempsInitial, TempsFinal);
                }
                else
                {
                   Temps = MathHelper.Clamp(Temps + (Temps�coul�DepuisMAJ * H�ros.SLOW_PROJ_SCALE * �CHELLE_TEMPS), TempsInitial, TempsFinal);
                }
                Temps�coul�DepuisMAJ = 0;
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
            return ((-(0.5f)) * ACC�L�RATION_GRAVITATIONNELLE * t * t) + (Vitesse * (float)Math.Sin((double)AngleTheta) * t) + yi;

        }

        private void RecalculerMonde()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(�chelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        private float CalculerVitesseSelonPort�eMax(float port�eMax, float h, float g)
        {
            return (float)Math.Sqrt((port�eMax + h) * g) * port�eMax / (port�eMax + h);
        }

        private float CalculerAngleThetaSelonVitesse(float v, float port�eR�elle, float h, float g)
        {
            return (float)Math.Atan((-h * (v * v + g * h + (float)Math.Sqrt(v * v * v * v + 2 * v * v * g * h - port�eR�elle * port�eR�elle * g * g)) / (h * h + port�eR�elle * port�eR�elle) + g) / (v * (float)Math.Sqrt((v * v + g * h + (float)Math.Sqrt(v * v * v * v + 2 * v * v * g * h - port�eR�elle * port�eR�elle * g * g)) / (h * h + port�eR�elle * port�eR�elle))) * (v / ((float)Math.Sqrt((v*v + g * h + (float)Math.Sqrt(v * v * v * v + 2 * v * v * g * h - port�eR�elle * port�eR�elle * g * g)) / (h * h + port�eR�elle * port�eR�elle)) * port�eR�elle)));
        }

        private void Debugger()
        {
            Debug.Clear();
            Debug.WriteLine("Enabled = " + (this.Enabled).ToString() + "; Visible = " + (this.Visible).ToString());
            Debug.WriteLine("Temps initial r�el:" + TempsInitialR�el.ToString());
            Debug.WriteLine("Temps initial:     " + TempsInitial.ToString());
            Debug.WriteLine("Temps final:       " + TempsFinal.ToString());
            Debug.WriteLine("Temps max:         " + TempsMax.ToString());
            Debug.WriteLine("Position initiale: " + PositionInitiale.ToString());
            Debug.WriteLine("Position finale:   " + PositionFinale.ToString());
            Debug.WriteLine("Position max:      " + PositionMax.ToString());
            Debug.WriteLine("Port�e:            " + Port�e.ToString());
            Debug.WriteLine("-------------------------------------------------");
            Debug.WriteLine("Position:          " + Position.ToString());
            Debug.WriteLine("Temps:             " + Temps.ToString());
            Debug.WriteLine("Vitesse initiale:  " + Vitesse.ToString());
            Debug.WriteLine("Angle theta:       " + AngleTheta.ToString());
            Debug.WriteLine("Angle beta:        " + AngleBeta.ToString());
        }

        // Informations sur la trajectoire
        private float CalculerTempsInitialR�el(float v, float theta, float a, float hauteurInitiale)
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
            return ((CalculerTempsFinal(v, theta, a, hauteurInitiale) - CalculerTempsInitialR�el(v, theta, a, hauteurInitiale)) / 2) + CalculerTempsInitialR�el(v, theta, a, hauteurInitiale);
        }

        private float CalculerPort�e(float v, float theta, float a, float hauteurInitiale)
        {
            return (v * v * (float)Math.Sin(theta) * (float)Math.Cos(theta) + v * (float)Math.Cos(theta) * (float)Math.Sqrt(v * v * Math.Sin(theta) * Math.Sin(theta) + 2 * a * hauteurInitiale)) / a;
        }

        public static float DistanceEntreDeuxPoints3D(Vector3 pt1, Vector3 pt2)
        {
            return (float)Math.Sqrt(Math.Pow((pt2.X - pt1.X), 2) + Math.Pow((pt2.Y - pt1.Y), 2) + Math.Pow((pt2.Z - pt1.Z), 2));
        }
    }
}
