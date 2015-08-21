using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class Monstre : Combattant
    {
        const float INTERVALLE_D�PLACEMENT_MIN = 2.5f; //intervalle de d�placement minimal
        const float INTERVALLE_D�PLACEMENT_DELTA = 4f; //intervalle de d�placement max = min + delta

        const int MAX_ZONE_PATROUILLE = 80;
        const int MIN_ZONE_PATROUILLE = 40;

        const float CHARM_LEASH_DISTANCE = 10; //ne doit pas �tre plus haut que l'aggrorange

        float IntervalleD�placement { get; set; }
        float TempsDepuisD�placement { get; set; }

        float TempsDepuisMAJ { get; set; }

        protected Rectangle ZonePatrouille { get; set; }

        protected H�ros Joueur { get; set; }

        protected HealthBar IndicateurVie { get; set; }

        public float AggroRange { get; private set; }
        public int ID { get; protected set; }
        public int Niveau { get; protected set; }

        private bool Charmed { get; set; }
        private float CharmTimer { get; set; }
        private float CharmDuration { get; set; }


        public Monstre(RPG jeu, Sc�neDeJeu sc�neJeu, H�ros joueur, string nomMod�le, float scale, float �chelleBox, Vector3 positionInitiale, Vector3 rotationInitiale, Vector3 rotationOffset,
                          string name, float vitesseD�placementInitiale, float vitesseRotationInitiale, bool peutBougerEnTournant, float ptsVie,
                          int ptsD�fense, int ptsAttaque, int deltaDamage, float attackSpeed, bool isRange, float range, float aggrorange, int niveau, int id)
           : base(jeu, sc�neJeu, nomMod�le, scale, �chelleBox, positionInitiale, rotationInitiale, rotationOffset, name, vitesseD�placementInitiale, vitesseRotationInitiale,
                   peutBougerEnTournant, ptsVie, ptsD�fense, ptsAttaque, deltaDamage, attackSpeed, isRange, range)
        {
            Joueur = joueur;
            AggroRange = Math.Max(aggrorange, CHARM_LEASH_DISTANCE);
            int hauteur = Jeu.G�n�rateurAl�atoire.Next(MIN_ZONE_PATROUILLE, MAX_ZONE_PATROUILLE + 1);
            int largeur = Jeu.G�n�rateurAl�atoire.Next(MIN_ZONE_PATROUILLE, MAX_ZONE_PATROUILLE + 1);
            ZonePatrouille = new Rectangle((int)(PositionCoord.X - (largeur / 2)), (int)(PositionCoord.Y - (hauteur / 2)), largeur, hauteur);
            ID = id;
            Niveau = niveau;
        }

        public override void Initialize()
        {
            TempsDepuisMAJ = 0;
            IntervalleD�placement = INTERVALLE_D�PLACEMENT_MIN + (float)Jeu.G�n�rateurAl�atoire.NextDouble() * INTERVALLE_D�PLACEMENT_DELTA;
            IndicateurVie = Sc�neJeu.BarresManager.AjouterBarreDeVie(this);
            Charmed = false;
            CharmTimer = 0;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float temps�coul� = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsDepuisMAJ += temps�coul�;
            if (TempsDepuisMAJ >= Jeu.INTERVALLE_MAJ)
            {
                GestionCharm();
                TempsDepuisD�placement += TempsDepuisMAJ;
                GestionAttaque();
                if (Cible != null && Vector2.Distance(PositionCoord, Cible.PositionCoord) > AggroRange*3)
                {
                    GestionUnleash();
                    Cible = null;
                    ActionActuelle = Action.ATTENDRE;
                }
                

                if (TempsDepuisD�placement >= IntervalleD�placement && ActionActuelle == Action.ATTENDRE)
                {
                    ActionActuelle = Action.D�PLACER;
                    Vector2 positionCible = new Vector2(Jeu.G�n�rateurAl�atoire.Next(ZonePatrouille.X, ZonePatrouille.X + ZonePatrouille.Width + 1),
                                                    Jeu.G�n�rateurAl�atoire.Next(ZonePatrouille.Y, ZonePatrouille.Y + ZonePatrouille.Height + 1));
                    D�buterD�placement(positionCible);
                    IntervalleD�placement = INTERVALLE_D�PLACEMENT_MIN + (float)Jeu.G�n�rateurAl�atoire.NextDouble() * INTERVALLE_D�PLACEMENT_DELTA;
                    TempsDepuisD�placement = 0;
                }
                V�rifier�tatBarreDeVie();
                V�rifier�tatH�ros();
                TempsDepuisMAJ = 0;
            }
            base.Update(gameTime);
        }

        protected virtual void GestionUnleash()
        { }


        protected virtual void GestionCharm()
        {
            if (CharmTimer > 0)
            {
                CharmTimer -= TempsDepuisMAJ;
                if (CharmTimer <= 0)
                {
                    Charmed = false;
                }
            }

        }

        protected virtual void GestionAttaque()
        {
           float distance = (Cible == null) ? AggroRange : (Vector2.Distance(Cible.PositionCoord, PositionCoord));
           if (Charmed && distance > CHARM_LEASH_DISTANCE)
           {

              float distancetemp;
              Monstre cibletemp = null;
              foreach (Monstre m in Sc�neJeu.MonstManager.ListeMonstres)
              {
                 if (!Monstre.Equals(m, this))
                 {
                    distancetemp = Vector2.Distance(PositionCoord, m.PositionCoord);
                    if (distancetemp < distance)
                    {
                       distance = distancetemp;
                       cibletemp = m;
                    }
                 }
              }
              if (cibletemp != null)
              {
                 Vector2 vDirection = Vector2.Subtract(cibletemp.PositionCoord, PositionCoord);
                 vDirection = Vector2.Normalize(vDirection);
                 D�buterRotation(MathHelper.WrapAngle(vDirection.X == 0 ? MathHelper.PiOver2 * Math.Sign(-vDirection.Y) : ((float)Math.Atan(-vDirection.Y / vDirection.X) + (Math.Sign(vDirection.X) - 1) * MathHelper.Pi / 2)));

                 D�buterAttaque(cibletemp);
              }
           }
           else
           {
              if (Vector2.Distance(PositionCoord, Joueur.PositionCoord) < AggroRange)
              {
                 Vector2 vDirection = Vector2.Subtract(Joueur.PositionCoord, PositionCoord);
                 vDirection = Vector2.Normalize(vDirection);
                 D�buterRotation(MathHelper.WrapAngle(vDirection.X == 0 ? MathHelper.PiOver2 * Math.Sign(-vDirection.Y) : ((float)Math.Atan(-vDirection.Y / vDirection.X) + (Math.Sign(vDirection.X) - 1) * MathHelper.Pi / 2)));
                 D�buterAttaque(Joueur);
              }
           }
        }

        protected override void G�r�rCouleurCharm(ref BasicEffect effet)
        {
           if (Charmed)
           {
              effet.EmissiveColor = new Vector3(0, 0, 0.2f + (0.8f * (CharmTimer / CharmDuration)));
           }
           base.G�r�rCouleurCharm(ref effet);
        }

        protected override void G�rerSurlignage(ref BasicEffect effet)
        {
           if (IndicateurVie.Visible)
           {
              effet.AmbientLightColor = new Vector3(0.8f, 0.8f, 0.65f);
           }
           base.G�rerSurlignage(ref effet);
        }

        protected virtual void V�rifier�tatH�ros()
        {
            if (Joueur.Reset)
            {
                PtsVie = PtsVieMax;
            }
        }

        private void V�rifier�tatBarreDeVie()
        {
            if (Monstre.Equals(this, Sc�neJeu.GestionInput3D.GetSourisBoxIntercept(Sc�neJeu.MonstManager.ListeMonstres)))
            {
                IndicateurVie.Visible = true;
            }
            else
            {
                IndicateurVie.Visible = false;
            }
        }

        public override void PerdrePointsDeVie(int D�gats)
        {
           if (Cible == null)
           {
              Cible = Sc�neJeu.BaldorLeBrave;
              ActionActuelle = Action.ATTAQUER;
           }
           PtsVie = PtsVie - Math.Max((D�gats - PtsD�fense), 0);
           if (PtsVie <= 0)
           {
              Sc�neJeu.BaldorLeBrave.AugmenterExp(Niveau);
           }
        }

        public virtual void BecomeCharmed(float duration)
        {
            Charmed = true;
            Cible = null;
            CharmTimer = duration;
            CharmDuration = duration;
        }
    }
}
