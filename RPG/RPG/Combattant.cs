using System;
using Microsoft.Xna.Framework;

namespace AtelierXNA
{

    public class Combattant : ÊtreVivant
    {
        float TempsDepuisMAJ { get; set; }
        float TempsDepuisAttaque { get; set; }

        public float AttackSpeed { get; protected set; }
        public float Range { get; private set; }
        public float PtsVie { get; protected set; }
        public float PtsVieMax { get; protected set; }
        public int PtsAttaque { get; protected set; }
        public int PtsDéfense { get; protected set; }
        public int DeltaDamage { get; protected set; }

        public bool IsRange { get; private set; }
        protected Combattant Cible { get; set; }
        public bool ToRemove { get { return PtsVie <= 0; } }


        public Combattant(RPG game, ScèneDeJeu scèneJeu, String nomModèle, float scale, float échelleBox, Vector3 positionInitiale, Vector3 rotationInitiale, Vector3 rotationOffset,
                          string name, float vitesseDéplacementInitiale, float vitesseRotationInitiale, bool peutBougerEnTournant, float ptsVie,
                          int ptsDéfense, int ptsAttaque, int deltaDamage, float attackSpeed, bool isRange, float range)
           : base(game, scèneJeu, nomModèle, scale, échelleBox,positionInitiale, rotationInitiale, rotationOffset, name, vitesseDéplacementInitiale, vitesseRotationInitiale,
                   peutBougerEnTournant)
        {
            PtsVie = ptsVie;
            PtsVieMax = ptsVie;
            PtsDéfense = PtsDéfense;
            PtsAttaque = ptsAttaque;
            DeltaDamage = deltaDamage;
            AttackSpeed = attackSpeed;
            Range = range;
            IsRange = isRange;
        }

        public override void Initialize()
        {
            TempsDepuisMAJ = 0;
  
            base.Initialize();
        }
        public override void Update(GameTime gameTime)
        {
           if (TempsDepuisMAJ >= Jeu.INTERVALLE_MAJ)
            {

                TempsDepuisAttaque += TempsDepuisMAJ;
                if (Cible != null && ActionActuelle == Action.ATTAQUER)
                {
                    if (Vector3.Distance(Cible.Position, Position) <= Range)
                    {
                        if (TempsDepuisAttaque > AttackSpeed)
                        {
                            if (!IsRange)
                            {
                                AttaquerMelee();
                            }
                            else
                            {
                                AttaquerRanged();
                            }
                            TempsDepuisAttaque = 0;
                        }

                    }
                    else
                    {
                        Vector3 direction = Vector3.Normalize(Cible.Position - Position);
                        DébuterDéplacement(Cible.Position - direction * (Range-0.01f));
                    }
                }
                TempsDepuisMAJ = 0;
            }
            ScèneJeu.CollisionManager.CheckCombattantHit(this);
            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsDepuisMAJ += tempsÉcoulé;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        protected virtual void AttaquerMelee()
        {
            int Dégats = PtsAttaque + Jeu.GénérateurAléatoire.Next(0, DeltaDamage + 1);
            Cible.PerdrePointsDeVie(Dégats);
            if (Cible.PtsVie <= 0)
            {
                Cible = null;
                PositionCible = PositionCoord;

            }
            ActionActuelle = Action.ATTENDRE;
        }

        protected virtual void AttaquerRanged()
        {

            int Dégats = Math.Max((PtsAttaque + Jeu.GénérateurAléatoire.Next(0, DeltaDamage + 1)) - Cible.PtsDéfense, 0);
            ScèneJeu.ProjManager.CréerProjectile(this, Cible.Position, Dégats, Range);
            if (Cible.PtsVie <= 0)
            {
                Cible = null;
                PositionCible = PositionCoord;
                ActionActuelle = Action.ATTENDRE;
            }
        }
        public virtual void PerdrePointsDeVie(int Dégats)
        {
            PtsVie = PtsVie - Math.Max((Dégats-PtsDéfense), 0);
        }

        public virtual void DébuterAttaque(Combattant cible)
        {
            ActionActuelle = Action.ATTAQUER;
            Cible = cible;
            PositionCible = PositionCoord;
        }
    }
}
