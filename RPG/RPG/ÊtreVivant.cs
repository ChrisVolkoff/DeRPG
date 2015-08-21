using System;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{
    public enum Action { ATTENDRE, ATTAQUER, DÉPLACER } 
    public class ÊtreVivant : ObjetDeBasePhysique//, IDéplacable
    {
        public const float PRÉCISION_MOUVEMENT = 0.5f;

        public string Name { get; private set; }
        float TempsDepuisMAJ { get; set; }
        

        public float VitesseDéplacement { get; protected set; }
        public float VitesseDéplacementInitiale { get; private set; }
        public float VitesseRotation { get; protected set; }
        public float VitesseRotationInitiale { get; private set; }
        bool PeutBougerEnTournant { get; set; }

        /// <summary>
        ///Ceci représente l'ordre actuel de l'être, et non ce qu'il fait en ce moment même 
        ///(s'il est en route pour se battre, son ordre est d'attaquer, même s'il est présentement 
        ///en train de se déplacer vers sa cible).
        /// </summary>
        public Action ActionActuelle { get; protected set; } 

        Vector2 positionCoord;
        public Vector2 PositionCoord
        {
            get { return positionCoord; }
            protected set
            {
                positionCoord = value;
                Position = new Vector3(positionCoord.X, ScèneJeu.MapManager.CalculerHauteurPoint(positionCoord) + 1f, positionCoord.Y); //ne pas oublier d'enlever le 4.1f
            }
        }

        float AngleDéplacement { get; set; }
        Vector2 VecteurDirection { get; set; }

        public float RotationCible { get; set; }
        public Vector2 PositionCible { get; set; }


        public ÊtreVivant(RPG game, ScèneDeJeu scèneJeu, String nomModèle, float scale, float échelleBox, Vector3 positionInitiale, Vector3 rotationInitiale, Vector3 rotationOffset,
                          string name, float vitesseDéplacementInitiale, float vitesseRotationInitiale, bool peutBougerEnTournant)
           : base(game, scèneJeu, nomModèle, scale, échelleBox, rotationInitiale, rotationOffset, positionInitiale)
        {
            PositionCoord = new Vector2(positionInitiale.X, positionInitiale.Z);
            Name = name;
            VitesseDéplacementInitiale = vitesseDéplacementInitiale;
            VitesseRotationInitiale = vitesseRotationInitiale;
            VitesseDéplacement = vitesseDéplacementInitiale;
            VitesseRotation = vitesseRotationInitiale;
            PeutBougerEnTournant = peutBougerEnTournant;
        }
        public override void Initialize()
        {
            TempsDepuisMAJ = 0;
            ActionActuelle = Action.ATTENDRE;
            VitesseDéplacement = VitesseDéplacementInitiale;
            VitesseRotation = VitesseRotationInitiale;
            PositionCible = PositionCoord;
            RotationCible = Rotation.Y;
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsDepuisMAJ += tempsÉcoulé;
            if (TempsDepuisMAJ > Jeu.INTERVALLE_MAJ)
            {
                DéplacementConstant();
                RotationConstante();
                EffectuerMAJ();
                TempsDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        protected override TimeSpan GérerTimeScale(GameTime gameTime)
        {
           TimeSpan retour;
           if (ActionActuelle == Action.ATTENDRE)
           {
              retour = new TimeSpan((int)(gameTime.ElapsedGameTime.Ticks * 0));
           }
           else
           {
              retour = new TimeSpan((int)(gameTime.ElapsedGameTime.Ticks * 1.5f));
           }

           return retour;
        }
        private void EffectuerMAJ()
        {
            Monde = Matrix.Identity;
            Monde *= Matrix.CreateScale(Échelle);
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
        }

        private void DéplacementConstant() //Note: les monstres doivent arrêter de bouger quand ils atteignent leur cible
        {
            Vector2 anciennePosition = PositionCoord;
            Vector3 anciennePosition3D = GetPositionFromPositionCoord(anciennePosition);
            Vector2 newPosition = GetNewPosition();
            Vector3 newPosition3D = GetPositionFromPositionCoord(newPosition);

            if (ScèneJeu.CollisionManager.CheckCollisionWithOtherPeople(this, anciennePosition3D, newPosition3D) || ScèneJeu.CollisionManager.CheckCollisionWithDoodads(this, anciennePosition3D, newPosition3D)) // || distance(newpos, posobj) > dist(posactuelle, posobj)
            {
                PositionCoord = anciennePosition;
            }
            else
            {
                PositionCoord = newPosition;
            }


        }

        protected Vector2 GetNewPosition()
        {
            Vector2 newPositionCoord;
            float Distance = Vector2.Distance(PositionCible, PositionCoord);
            if ((Distance > 0.01f) && (PeutBougerEnTournant || (Rotation.Y < AngleDéplacement + 0.1f && Rotation.Y > AngleDéplacement - 0.1f)))
            {
                newPositionCoord = Vector2.Add(PositionCoord, VecteurDirection * MathHelper.Min((VitesseDéplacement * TempsDepuisMAJ), Distance));
            }
            else
            {
                if (Distance < 0.01f)
                {
                    newPositionCoord = new Vector2(PositionCoord.X, PositionCoord.Y);
                    if (ActionActuelle == Action.DÉPLACER)
                    {
                        ActionActuelle = Action.ATTENDRE;
                    }
                }
                else // aucun changement
                {
                    newPositionCoord = PositionCoord;
                }
            }
           
            return newPositionCoord;
        }

        private void RotationConstante()
        {
            float rotation = Rotation.Y;
            if (!(rotation < RotationCible + 0.1f && rotation > RotationCible - 0.1f))
            {
                rotation += ((MathHelper.WrapAngle(RotationCible - rotation) >= 0 ? VitesseRotation : -VitesseRotation) * TempsDepuisMAJ);
            }
            Rotation = new Vector3(Rotation.X, MathHelper.WrapAngle(rotation), Rotation.Z);
        }


        public void DébuterRotation(float rotationCible)
        {
            RotationCible = MathHelper.WrapAngle(rotationCible);
        }

        public void DébuterDéplacement(Vector2 positionCible)
        {
            PositionCible = new Vector2(positionCible.X, positionCible.Y); //Penser à faire la projection de la position sur le terrain (toujours utiliser y=0 jusqu'à ce que vienne le temps de faire le draw, ou on met y=la hauteur du terrain)
            VecteurDirection = Vector2.Subtract(PositionCible, PositionCoord);
            VecteurDirection = Vector2.Normalize(VecteurDirection);
            try
            {
                AngleDéplacement = MathHelper.WrapAngle(VecteurDirection.X == 0 ? MathHelper.PiOver2 * Math.Sign(-VecteurDirection.Y) : ((float)Math.Atan(-VecteurDirection.Y / VecteurDirection.X) + (Math.Sign(VecteurDirection.X) - 1) * MathHelper.Pi / 2));
                //AngleDéplacement = MathHelper.WrapAngle(VecteurDirection.Z == 0 ? MathHelper.PiOver2 * Math.Sign(VecteurDirection.X) : ((float)Math.Atan(VecteurDirection.X / VecteurDirection.Z)+(Math.Sign(VecteurDirection.Z)-1)*MathHelper.Pi/2));

                if (!(Rotation.Y < AngleDéplacement + 0.05f && Rotation.Y > AngleDéplacement - 0.05f))
                {
                    DébuterRotation(AngleDéplacement);
                }
            }
            catch { }
        }

        public void DébuterDéplacement(Vector3 positionCible)
        {
            PositionCible = new Vector2(positionCible.X, positionCible.Z); //Penser à faire la projection de la position sur le terrain (toujours utiliser y=0 jusqu'à ce que vienne le temps de faire le draw, ou on met y=la hauteur du terrain)
            VecteurDirection = Vector2.Subtract(PositionCible, PositionCoord);
            VecteurDirection = Vector2.Normalize(VecteurDirection);
            try
            {
                AngleDéplacement = MathHelper.WrapAngle(VecteurDirection.X == 0 ? MathHelper.PiOver2 * Math.Sign(-VecteurDirection.Y) : ((float)Math.Atan(-VecteurDirection.Y / VecteurDirection.X) + (Math.Sign(VecteurDirection.X) - 1) * MathHelper.Pi / 2));
                //AngleDéplacement = MathHelper.WrapAngle(VecteurDirection.Z == 0 ? MathHelper.PiOver2 * Math.Sign(VecteurDirection.X) : ((float)Math.Atan(VecteurDirection.X / VecteurDirection.Z)+(Math.Sign(VecteurDirection.Z)-1)*MathHelper.Pi/2));

                if (!(Rotation.Y < AngleDéplacement + 0.05f && Rotation.Y > AngleDéplacement - 0.05f))
                {
                    DébuterRotation(AngleDéplacement);
                }
            }
            catch { }
        }

        protected Vector3 GetPositionFromPositionCoord(Vector2 positionCoord)
        {
            return new Vector3(positionCoord.X, ScèneJeu.MapManager.CalculerHauteurPoint(positionCoord) + 1f, positionCoord.Y);
        }
    }
}
