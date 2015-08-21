using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
    public class Boss : Monstre
    {
        const int NB_MINIONS = 4;
        const float DiSTANCE_MINIONS = 5f;
        float VieEntracte { get; set; }
        int PtsAttaqueInitial { get; set; }
        Vector3 PositionInitiale { get; set; }
        bool Tampon { get; set; }
        bool EstInvincible { get; set; }
        List<int> ListeIDMinions { get; set; }
        public bool HasAggroed { get; private set; }

        public Boss(RPG jeu, ScèneDeJeu scèneJeu, Héros joueur, String nomModèle, float scale, float échelleBox, Vector3 positionInitiale, Vector3 rotationInitiale, Vector3 rotationOffset,
                          string name, float vitesseDéplacementInitiale, float vitesseRotationInitiale, bool peutBougerEnTournant, float ptsVie,
                          int ptsDéfense, int ptsAttaque, int deltaDamage, float attackSpeed, bool isRange, float range, float aggrorange, int niveau, int id)
            : base(jeu, scèneJeu, joueur, nomModèle, scale, échelleBox, positionInitiale, rotationInitiale, rotationOffset, name, vitesseDéplacementInitiale, vitesseRotationInitiale,
                    peutBougerEnTournant, ptsVie, ptsDéfense, ptsAttaque, deltaDamage, attackSpeed, isRange, range, aggrorange, niveau, id)
        {
            PtsAttaqueInitial = ptsAttaque;
            PositionInitiale = positionInitiale;
            ListeIDMinions = new List<int>();
        }

        public override void Initialize()
        {
            VieEntracte = PtsVie / 3;
            Tampon = true;
            EstInvincible = false;
            ScèneJeu.MonstManager.ListeMonstres.Add(this);
            ID = ScèneJeu.MonstManager.GetID();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateMinionsID();
            VérifierÉtatMinions();
            VérifierÉtatHéros();
            Visible = (Vector3.Distance(Position, ScèneJeu.BaldorLeBrave.Position) < Caméra.DISTANCE_PLAN_ÉLOIGNÉ);
            if (PtsVie <= VieEntracte && Tampon)
            {
                CréérMinions();
                EstInvincible = true;
                Tampon = false;
                PtsAttaque *= 2;
                //Soundtrack.StartSoundCue(nomQuote) 
            }
            if (ToRemove)
            {
                Soundtrack.StartSoundCue("boss_death");
                Jeu.GestionScènes.ChangerDeScène(Scènes.Credits);
            }
            if (HasAggroed && Soundtrack.IsSongCueStopped)
            {
                Soundtrack.StartSongCueWithDistanceEffect("boss_battle_loop", "Battle", ScèneJeu.BaldorLeBrave.Position, Position, (AggroRange * 3));
            }
            else
            {
                if (HasAggroed)
                {
                    Soundtrack.UpdateSongCueDistanceEffect("Battle", ScèneJeu.BaldorLeBrave.Position, Position, AggroRange * 3);
                }
            }
            base.Update(gameTime);
        }

        protected override void GestionCharm() { } //boss ne peut pas être charmed

        protected override void GestionAttaque()
        {
            if (Vector2.Distance(PositionCoord, Joueur.PositionCoord) < AggroRange)
            {
                DébuterAttaque(Joueur);
            }
        }

        private void CréérMinions()
        {
            Soundtrack.StartSoundCue("boss_minion");

            for (int i = 0; i < NB_MINIONS; i++)
            {
                int id = ScèneJeu.MonstManager.GetID();
                ListeIDMinions.Add(id);
            }
            for (int i = 0; i < NB_MINIONS; ++i)
            {
                float angle = i * MathHelper.TwoPi / NB_MINIONS;
                Vector2 vDirection = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 positionminion = PositionCoord + vDirection * DiSTANCE_MINIONS;
                ScèneJeu.MonstManager.CréerMonstre(Joueur, "Cyclops\\terrorwurm", 0.06f, ScèneDeJeu.ÉCHELLE_BOX_WURM, new Vector3(positionminion.X, positionminion.Y, Position.Z),
                                                          new Vector3(0, 0, 0), new Vector3(0, -MathHelper.PiOver2, 0), "Minion", 15f, MathHelper.Pi * 4, false, 21, 3, 6, 4, 1f, false, 15, 80, 6, ListeIDMinions[0]);
            //"unicorn", 8f, 1f,
            }
        }
        public override void PerdrePointsDeVie(int Dégats)
        {
            if (!EstInvincible)
            {
                base.PerdrePointsDeVie(Dégats);
            }
        }
        private void VérifierÉtatMinions()
        {
            if (ListeIDMinions.Count <= 0)
            {
                EstInvincible = false;
            }
        }

        private void UpdateMinionsID()    // Update ListeID
        {
            for (int i = ListeIDMinions.Count - 1; i >= 0; --i)
            {
                if (!ScèneJeu.MonstManager.IsIDActive(ListeIDMinions[i]))
                {
                    ListeIDMinions.RemoveAt(i);
                }
            }
        }

        protected override void VérifierÉtatHéros()
        {
            if (Joueur.Reset)
            {
                PtsVie = PtsVieMax;
                PtsAttaque = PtsAttaqueInitial;
                Tampon = true;
                EstInvincible = false;
                foreach (int id in ListeIDMinions)
                {
                    ScèneJeu.MonstManager.ListeMonstres.Remove(ScèneJeu.MonstManager.GetMonsterWithID(id));
                }
            }
        }

        public override void BecomeCharmed(float duration) { }

        public override void Draw(GameTime gameTime)
        {
            if (Animated)
            {
                Matrix[] bones = AnimPlayer.GetSkinTransforms();

                // Compute camera matrices.
                Matrix view = ScèneJeu.CaméraJeu.Vue;

                Matrix projection = ScèneJeu.CaméraJeu.Projection;

                // Render the skinned mesh.
                foreach (ModelMesh mesh in Modèle.Meshes)
                {
                    Matrix mondeLocal = TransformationsModèle[mesh.ParentBone.Index] * GetMonde();
                    foreach (SkinnedEffect effect in mesh.Effects)
                    {
                        effect.SetBoneTransforms(bones);

                        effect.View = view;
                        effect.Projection = projection;
                        effect.World = mondeLocal;

                        effect.EnableDefaultLighting();
                    }

                    mesh.Draw();
                }
            }
            else
            {
                foreach (ModelMesh maille in Modèle.Meshes)
                {
                    Vector3[] oldColor = new Vector3[maille.MeshParts.Count];
                    int i = 0;
                    Matrix mondeLocal = TransformationsModèle[maille.ParentBone.Index] * GetMonde();
                    foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                    {
                        BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                        oldColor[i] = effet.EmissiveColor;
                        effet.EnableDefaultLighting();
                        GérerSurlignage(ref effet);
                        GérérCouleurCharm(ref effet);
                        effet.Projection = ScèneJeu.CaméraJeu.Projection;
                        effet.View = ScèneJeu.CaméraJeu.Vue;
                        effet.World = mondeLocal;
                        i++;
                    }
                    maille.Draw();
                    i = 0;
                    foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                    {
                        BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                        effet.EmissiveColor = oldColor[i];
                        i++;
                    }
                }
            }
            //Draw boxes
            if (DrawBoxes)
            {
                //Box effect
                if (lineEffect == null) // Comme ça on a qu'à le créer une seule fois (et on ne peut le créer dans le Initialize())
                {
                    lineEffect = new BasicEffect(ScèneJeu.GraphicsDevice);
                    lineEffect.LightingEnabled = false;
                    lineEffect.TextureEnabled = false;
                    lineEffect.VertexColorEnabled = true;
                }
                foreach (BoundingBoxBuffers bb in BoxDrawList)
                {
                    DrawBoundingBox(bb, lineEffect, ScèneJeu.GraphicsDevice, ScèneJeu.CaméraJeu.Vue, ScèneJeu.CaméraJeu.Projection);
                }
            }
        }

        public override void DébuterAttaque(Combattant cible)
        {
            if (!HasAggroed)
            {
                HasAggroed = true;
                Soundtrack.StartSongCue("boss_battle_start");
            }
            base.DébuterAttaque(cible);
        }

        protected override void GestionUnleash()
        {
            if (HasAggroed)
            {
                HasAggroed = false;
                Soundtrack.StopSong();
                ScèneJeu.MapManager.StartMusic();
            }
            base.GestionUnleash();
        }
    }
}