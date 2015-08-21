using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
    public class Héros : Combattant
    {
        public const float INTERVALLE_MAJ_RESET = 2f;

        const int INCRÉMENT_ATTAQUE_MIN = 1;
        const int INCRÉMENT_ATTAQUE_MAX = 3;
        const int INCRÉMENT_DEFENSE_MIN = 1;
        const int INCRÉMENT_DEFENSE_MAX = 2;
        const int INCRÉMENT_VIE_MIN = 8;
        const int INCRÉMENT_VIE_MAX = 15;
        const int INCRÉMENT_RESSOURCE_MIN = 4;
        const int INCRÉMENT_RESSOURCE_MAX = 7;

        const float RANGE = 35f;

        const int EXP_PREMIER_NIVEAU = 200;

        const int FIREBALL_MANA_COST = 2;
        const int FIREBALL_DAMAGE = 10;
        const float FIREBALL_RANGE = 200;
        const int NOVA_MANA_COST = 8;
        const int NOVA_DAMAGE = 15;
        const float NOVA_HAUTEUR = 4f;
        const int NOVA_DURÉE = 1000; //en ms
        const float NOVA_RAYON = 70;
        const float CHARM_DURÉE = 6;
        const int CHARM_MANA_COST = 20;
        const float SLOW_PROJ_INSTANT_MANA_COST = 4;
        const float SLOW_PROJ_TIME_MANA_COST = 1.5f;
        public const float SLOW_PROJ_SCALE = 0.2f;

        public bool IsSlowingProj { get; private set; }
        bool Cooldown { get; set; }

        string NomTextureNova { get; set; }
        private Nova NovaActuel { get; set; }

        const float POURCENTAGE_REGEN_MANA = 0.03f;
        const float POURCENTAGE_REGEN_VIE = 0.018f;

        public float PtsRessourceMax { get; protected set; }
        public float PtsRessource { get; protected set; }
        public float PtsExp { get; protected set; }
        public int Niveau { get; protected set; }
        float TempsDepuisMAJ { get; set; }
        float TempsDepuisMAJRESET { get; set; }
        float TempsDepuisRegen { get; set; }
        public float ExpProchainNiveau { get; private set; }
        float IncrémentRegenVie { get; set; }
        float IncrémentRegenRessource { get; set; }

        Vector2 PositionCoordInitiale { get; set; }
        public bool Reset { get; private set; }

        public Héros(RPG jeu, ScèneDeJeu scèneJeu, String nomModèle, float scale, float échelleBox, Vector3 positionInitiale, Vector3 rotationInitiale, Vector3 rotationOffset,
                          string name, float vitesseDéplacementInitiale, float vitesseRotationInitiale, bool peutBougerEnTournant, float ptsVie, float ptsRessource,
                          int ptsDéfense, int ptsAttaque, int deltaDamage, float attackSpeed, string nomtextureNova)
            : base(jeu, scèneJeu, nomModèle, scale, échelleBox, positionInitiale, rotationInitiale, rotationOffset, name, vitesseDéplacementInitiale, vitesseRotationInitiale,
                   peutBougerEnTournant, ptsVie, ptsDéfense, ptsAttaque, deltaDamage, attackSpeed, false, RANGE)
        {
            PositionCoordInitiale = new Vector2(positionInitiale.X, positionInitiale.Z);
            PtsRessourceMax = ptsRessource;
            PtsVieMax = ptsVie;
            PtsRessource = ptsRessource;
            NomTextureNova = nomtextureNova;
        }

        public override void Initialize()
        {
            PtsExp = 0;
            TempsDepuisMAJ = 0;
            TempsDepuisMAJRESET = 0;
            TempsDepuisRegen = 0;
            IncrémentRegenVie = PtsVieMax * POURCENTAGE_REGEN_VIE;
            IncrémentRegenRessource = PtsRessourceMax * POURCENTAGE_REGEN_MANA;
            Reset = false;
            Niveau = 1;
            Cooldown = false;

            ExpProchainNiveau = EXP_PREMIER_NIVEAU;

            base.Initialize();
        }
        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void AttaquerMelee()
        {

            Soundtrack.StartSoundCue("melee_swing");

            int Dégats = (PtsAttaque + Jeu.GénérateurAléatoire.Next(0, DeltaDamage + 1));
            Cible.PerdrePointsDeVie(Dégats);
            if (Cible.PtsVie <= 0)
            {
                Cible = null;
            }
            ActionActuelle = Action.ATTENDRE;
        }

        public void AugmenterExp(int niveauMonstre)
        {
            PtsExp += 20 * niveauMonstre;
            VérifierNiveau();
        }

        private void VérifierNiveau()
        {
            if (PtsExp >= ExpProchainNiveau)
            {
                PtsExp = 0;
                PtsAttaque += Jeu.GénérateurAléatoire.Next(INCRÉMENT_ATTAQUE_MIN, INCRÉMENT_ATTAQUE_MAX + 1);
                PtsDéfense += Jeu.GénérateurAléatoire.Next(INCRÉMENT_DEFENSE_MIN, INCRÉMENT_DEFENSE_MAX + 1);
                PtsVieMax += Jeu.GénérateurAléatoire.Next(INCRÉMENT_VIE_MIN, INCRÉMENT_VIE_MAX + 1);
                PtsVie = PtsVieMax;
                PtsRessourceMax += Jeu.GénérateurAléatoire.Next(INCRÉMENT_RESSOURCE_MIN, INCRÉMENT_RESSOURCE_MAX + 1);
                PtsRessource = PtsRessourceMax;
                ExpProchainNiveau = CalculerExpProchainNiveau();
                IncrémentRegenVie = PtsVieMax * POURCENTAGE_REGEN_VIE;
                IncrémentRegenRessource = PtsRessourceMax * POURCENTAGE_REGEN_MANA;
                ++Niveau;

                Soundtrack.StartSoundCue("levelup");
            }
        }

        private float CalculerExpProchainNiveau()
        {
            return ExpProchainNiveau * 2 + 100;
        }

        public override void PerdrePointsDeVie(int Dégats)
        {
            base.PerdrePointsDeVie(Dégats);
            if (ToRemove && !Reset)
            {
                Reset = true;

                Soundtrack.StartSoundCue("nec_death");
            }
        }

        public override void Update(GameTime gameTime)
        {

            float tempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
            TempsDepuisMAJ += tempsÉcoulé;
            if (TempsDepuisMAJ > Jeu.INTERVALLE_MAJ && !Reset)
            {
                GestionRegen(TempsDepuisMAJ);
                GestionOrdres();
                GestionAttaqueMagique();
                TempsDepuisMAJ = 0;
            }
            if (Reset)
            {
                GestionReset(tempsÉcoulé);
            }
            base.Update(gameTime);
        }
        private void GestionRegen(float tempsÉcoulé)
        {
            PtsVie = MathHelper.Clamp(PtsVie + IncrémentRegenVie * tempsÉcoulé, PtsVie, PtsVieMax);
            PtsRessource = MathHelper.Clamp(PtsRessource + IncrémentRegenRessource * tempsÉcoulé, PtsRessource, PtsRessourceMax);
            TempsDepuisRegen = 0;
        }
        private void GestionOrdres()
        {

            if (ScèneJeu.GestionInput3D.ÉtatSouris.LeftButton == ButtonState.Pressed)
            {
                Monstre cibletemp = ScèneJeu.GestionInput3D.GetSourisBoxIntercept(ScèneJeu.MonstManager.ListeMonstres);
                if (cibletemp != null)
                {
                    ActionActuelle = Action.ATTAQUER;
                    Vector2 vDirection = Vector2.Subtract(cibletemp.PositionCoord, PositionCoord);
                    vDirection = Vector2.Normalize(vDirection);
                    DébuterRotation(MathHelper.WrapAngle(vDirection.X == 0 ? MathHelper.PiOver2 * Math.Sign(-vDirection.Y) : ((float)Math.Atan(-vDirection.Y / vDirection.X) + (Math.Sign(vDirection.X) - 1) * MathHelper.Pi / 2)));
                    DébuterAttaque(cibletemp);
                }
                else
                {
                    Vector3 positionCible = ScèneJeu.GestionInput3D.GetPositionSouris3d();
                    Vector2 positionCoordCible = new Vector2(positionCible.X, positionCible.Z);
                    if ((positionCoordCible.X != 0 || positionCoordCible.Y != 0) && (positionCoordCible.X != PositionCoord.X || positionCoordCible.Y != PositionCoord.Y))
                    {
                        ActionActuelle = Action.DÉPLACER;
                        DébuterDéplacement(positionCoordCible);
                    }

                }
            }
        }

        private void GestionAttaqueMagique()
        {
            if (ScèneJeu.GestionInput3D.EstNouvelleTouche(Keys.D1))
            {
                Vector3 clickSouris = ScèneJeu.GestionInput3D.GetPositionSouris3d();
                if (clickSouris != Vector3.Zero && Vector3.Distance(Position, clickSouris) <= FIREBALL_RANGE)
                {
                    if (ThrowManaSpell(FIREBALL_MANA_COST))
                    {
                        ActionActuelle = Action.ATTENDRE;
                        PositionCible = PositionCoord;
                        Vector2 vDirection = Vector2.Subtract(new Vector2(clickSouris.X, clickSouris.Z), PositionCoord);
                        vDirection = Vector2.Normalize(vDirection);
                        DébuterRotation(MathHelper.WrapAngle(vDirection.X == 0 ? MathHelper.PiOver2 * Math.Sign(-vDirection.Y) : ((float)Math.Atan(-vDirection.Y / vDirection.X) + (Math.Sign(vDirection.X) - 1) * MathHelper.Pi / 2)));

                        ScèneJeu.ProjManager.CréerProjectile(this, clickSouris, FIREBALL_DAMAGE, FIREBALL_RANGE);
                    }
                }
            }
            if (ScèneJeu.GestionInput3D.EstNouvelleTouche(Keys.D2))
            {
                if (NovaActuel == null)
                {
                    if (ThrowManaSpell(NOVA_MANA_COST))
                    {
                        ActionActuelle = Action.ATTENDRE;
                        PositionCible = PositionCoord;

                        NovaActuel = ScèneJeu.NovManager.AjouterNova(new Vector3(Position.X, Position.Y + NOVA_HAUTEUR, Position.Z), NOVA_RAYON, new TimeSpan(0, 0, 0, 0, NOVA_DURÉE), NOVA_DAMAGE, NomTextureNova);
                        Soundtrack.StartSoundCue("sorc_nova");
                    }
                }
                else
                {
                    if (NovaActuel.ToRemove)
                    {
                        NovaActuel = null;
                    }
                }
            }
            if (!IsSlowingProj && ScèneJeu.GestionInput3D.EstNouvelleTouche(Keys.D3) && ThrowManaSpell(SLOW_PROJ_INSTANT_MANA_COST))  // Toggle slowing proj
            {
                IsSlowingProj = true;
                ScèneJeu.HeadsUpDisplay.Spells[2].Toggled = true;
                Soundtrack.StartSoundCue("chrono_start");
            }
            else
            {
                if (IsSlowingProj)
                {
                    if (ScèneJeu.GestionInput3D.EstNouvelleTouche(Keys.D3) || !ThrowManaSpell((float)SLOW_PROJ_TIME_MANA_COST * TempsDepuisMAJ))
                    {
                        IsSlowingProj = false;
                        ScèneJeu.HeadsUpDisplay.Spells[2].Toggled = false;
                        Soundtrack.StopSound();
                    }
                }
            }
            if (IsSlowingProj && Soundtrack.IsSoundCueStopped)
            {
                Soundtrack.StartSoundCue("chrono_loop");
            }




            if (ScèneJeu.GestionInput3D.EstNouvelleTouche(Keys.D4))
            {
                if (PtsRessource >= NOVA_MANA_COST)
                {
                    Monstre cibletemp = ScèneJeu.GestionInput3D.GetSourisBoxIntercept(ScèneJeu.MonstManager.ListeMonstres);
                    if (cibletemp != null && !(cibletemp is Boss))
                    {
                        cibletemp.BecomeCharmed(CHARM_DURÉE);
                        PtsRessource -= CHARM_MANA_COST;
                        ActionActuelle = Action.ATTENDRE;
                        PositionCible = PositionCoord;
                        Vector2 vDirection = Vector2.Subtract(cibletemp.PositionCoord, PositionCoord);
                        vDirection = Vector2.Normalize(vDirection);
                        DébuterRotation(MathHelper.WrapAngle(vDirection.X == 0 ? MathHelper.PiOver2 * Math.Sign(-vDirection.Y) : ((float)Math.Atan(-vDirection.Y / vDirection.X) + (Math.Sign(vDirection.X) - 1) * MathHelper.Pi / 2)));
                        Soundtrack.StartSoundCue("confusion");
                    }
                }
                else
                {
                    Soundtrack.StartSoundCue("nec_moremana");
                }
            }

            // For demonstration purpposes

        }

        private void GestionReset(float tempsÉcoulé)
        {
            TempsDepuisMAJRESET += tempsÉcoulé;
            if (TempsDepuisMAJRESET > INTERVALLE_MAJ_RESET)
            {
                PtsVie = PtsVieMax;
                PtsRessource = PtsRessourceMax;
                PositionCoord = PositionCoordInitiale;
                PositionCible = PositionCoordInitiale;
                Reset = false;
                TempsDepuisMAJRESET = 0;

                Soundtrack.StartSoundCue("boss_win");

                Jeu.GestionScènes.ChangerDeScène(Scènes.Mort);
            }
        }

        public bool ThrowManaSpell(int cost)
        {
            bool hasBeenThrown = false;

            if (PtsRessource >= cost)
            {
                PtsRessource -= cost;
                hasBeenThrown = true;
            }
            else
            {
                Soundtrack.StartSoundCue("nec_moremana");
            }

            return hasBeenThrown;
        }

        public bool ThrowManaSpell(float cost)
        {
            bool hasBeenThrown = false;

            if (PtsRessource >= cost)
            {
                PtsRessource -= cost;
                hasBeenThrown = true;
            }
            else
            {
                Soundtrack.StartSoundCue("nec_moremana");
            }

            return hasBeenThrown;
        }

    }

}