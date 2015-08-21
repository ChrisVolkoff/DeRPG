using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class Monstre : Combattant
    {
        const float INTERVALLE_DÉPLACEMENT_MIN = 2.5f; //intervalle de déplacement minimal
        const float INTERVALLE_DÉPLACEMENT_DELTA = 4f; //intervalle de déplacement max = min + delta

        const int MAX_ZONE_PATROUILLE = 80;
        const int MIN_ZONE_PATROUILLE = 40;

        const float CHARM_LEASH_DISTANCE = 10; //ne doit pas être plus haut que l'aggrorange

        float IntervalleDéplacement { get; set; }
        float TempsDepuisDéplacement { get; set; }

        float TempsDepuisMAJ { get; set; }

        protected Rectangle ZonePatrouille { get; set; }

        protected Héros Joueur { get; set; }

        protected HealthBar IndicateurVie { get; set; }

        public float AggroRange { get; private set; }
        public int ID { get; protected set; }
        public int Niveau { get; protected set; }

        private bool Charmed { get; set; }
        private float CharmTimer { get; set; }
        private float CharmDuration { get; set; }


        public Monstre(RPG jeu, ScèneDeJeu scèneJeu, Héros joueur, string nomModèle, float scale, float échelleBox, Vector3 positionInitiale, Vector3 rotationInitiale, Vector3 rotationOffset,
                          string name, float vitesseDéplacementInitiale, float vitesseRotationInitiale, bool peutBougerEnTournant, float ptsVie,
                          int ptsDéfense, int ptsAttaque, int deltaDamage, float attackSpeed, bool isRange, float range, float aggrorange, int niveau, int id)
           : base(jeu, scèneJeu, nomModèle, scale, échelleBox, positionInitiale, rotationInitiale, rotationOffset, name, vitesseDéplacementInitiale, vitesseRotationInitiale,
                   peutBougerEnTournant, ptsVie, ptsDéfense, ptsAttaque, deltaDamage, attackSpeed, isRange, range)
        {
            Joueur = joueur;
            AggroRange = Math.Max(aggrorange, CHARM_LEASH_DISTANCE);
            int hauteur = Jeu.GénérateurAléatoire.Next(MIN_ZONE_PATROUILLE, MAX_ZONE_PATROUILLE + 1);
            int largeur = Jeu.GénérateurAléatoire.Next(MIN_ZONE_PATROUILLE, MAX_ZONE_PATROUILLE + 1);
            ZonePatrouille = new Rectangle((int)(PositionCoord.X - (largeur / 2)), (int)(PositionCoord.Y - (hauteur / 2)), largeur, hauteur);
            ID = id;
            Niveau = niveau;
        }

        public override void Initialize()
        {
            TempsDepuisMAJ = 0;
            IntervalleDéplacement = INTERVALLE_DÉPLACEMENT_MIN + (float)Jeu.GénérateurAléatoire.NextDouble() * INTERVALLE_DÉPLACEMENT_DELTA;
            IndicateurVie = ScèneJeu.BarresManager.AjouterBarreDeVie(this);
            Charmed = false;
            CharmTimer = 0;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsDepuisMAJ += tempsÉcoulé;
            if (TempsDepuisMAJ >= Jeu.INTERVALLE_MAJ)
            {
                GestionCharm();
                TempsDepuisDéplacement += TempsDepuisMAJ;
                GestionAttaque();
                if (Cible != null && Vector2.Distance(PositionCoord, Cible.PositionCoord) > AggroRange*3)
                {
                    GestionUnleash();
                    Cible = null;
                    ActionActuelle = Action.ATTENDRE;
                }
                

                if (TempsDepuisDéplacement >= IntervalleDéplacement && ActionActuelle == Action.ATTENDRE)
                {
                    ActionActuelle = Action.DÉPLACER;
                    Vector2 positionCible = new Vector2(Jeu.GénérateurAléatoire.Next(ZonePatrouille.X, ZonePatrouille.X + ZonePatrouille.Width + 1),
                                                    Jeu.GénérateurAléatoire.Next(ZonePatrouille.Y, ZonePatrouille.Y + ZonePatrouille.Height + 1));
                    DébuterDéplacement(positionCible);
                    IntervalleDéplacement = INTERVALLE_DÉPLACEMENT_MIN + (float)Jeu.GénérateurAléatoire.NextDouble() * INTERVALLE_DÉPLACEMENT_DELTA;
                    TempsDepuisDéplacement = 0;
                }
                VérifierÉtatBarreDeVie();
                VérifierÉtatHéros();
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
              foreach (Monstre m in ScèneJeu.MonstManager.ListeMonstres)
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
                 DébuterRotation(MathHelper.WrapAngle(vDirection.X == 0 ? MathHelper.PiOver2 * Math.Sign(-vDirection.Y) : ((float)Math.Atan(-vDirection.Y / vDirection.X) + (Math.Sign(vDirection.X) - 1) * MathHelper.Pi / 2)));

                 DébuterAttaque(cibletemp);
              }
           }
           else
           {
              if (Vector2.Distance(PositionCoord, Joueur.PositionCoord) < AggroRange)
              {
                 Vector2 vDirection = Vector2.Subtract(Joueur.PositionCoord, PositionCoord);
                 vDirection = Vector2.Normalize(vDirection);
                 DébuterRotation(MathHelper.WrapAngle(vDirection.X == 0 ? MathHelper.PiOver2 * Math.Sign(-vDirection.Y) : ((float)Math.Atan(-vDirection.Y / vDirection.X) + (Math.Sign(vDirection.X) - 1) * MathHelper.Pi / 2)));
                 DébuterAttaque(Joueur);
              }
           }
        }

        protected override void GérérCouleurCharm(ref BasicEffect effet)
        {
           if (Charmed)
           {
              effet.EmissiveColor = new Vector3(0, 0, 0.2f + (0.8f * (CharmTimer / CharmDuration)));
           }
           base.GérérCouleurCharm(ref effet);
        }

        protected override void GérerSurlignage(ref BasicEffect effet)
        {
           if (IndicateurVie.Visible)
           {
              effet.AmbientLightColor = new Vector3(0.8f, 0.8f, 0.65f);
           }
           base.GérerSurlignage(ref effet);
        }

        protected virtual void VérifierÉtatHéros()
        {
            if (Joueur.Reset)
            {
                PtsVie = PtsVieMax;
            }
        }

        private void VérifierÉtatBarreDeVie()
        {
            if (Monstre.Equals(this, ScèneJeu.GestionInput3D.GetSourisBoxIntercept(ScèneJeu.MonstManager.ListeMonstres)))
            {
                IndicateurVie.Visible = true;
            }
            else
            {
                IndicateurVie.Visible = false;
            }
        }

        public override void PerdrePointsDeVie(int Dégats)
        {
           if (Cible == null)
           {
              Cible = ScèneJeu.BaldorLeBrave;
              ActionActuelle = Action.ATTAQUER;
           }
           PtsVie = PtsVie - Math.Max((Dégats - PtsDéfense), 0);
           if (PtsVie <= 0)
           {
              ScèneJeu.BaldorLeBrave.AugmenterExp(Niveau);
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
