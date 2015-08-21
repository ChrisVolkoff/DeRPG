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

        public Boss(RPG jeu, Sc�neDeJeu sc�neJeu, H�ros joueur, String nomMod�le, float scale, float �chelleBox, Vector3 positionInitiale, Vector3 rotationInitiale, Vector3 rotationOffset,
                          string name, float vitesseD�placementInitiale, float vitesseRotationInitiale, bool peutBougerEnTournant, float ptsVie,
                          int ptsD�fense, int ptsAttaque, int deltaDamage, float attackSpeed, bool isRange, float range, float aggrorange, int niveau, int id)
            : base(jeu, sc�neJeu, joueur, nomMod�le, scale, �chelleBox, positionInitiale, rotationInitiale, rotationOffset, name, vitesseD�placementInitiale, vitesseRotationInitiale,
                    peutBougerEnTournant, ptsVie, ptsD�fense, ptsAttaque, deltaDamage, attackSpeed, isRange, range, aggrorange, niveau, id)
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
            Sc�neJeu.MonstManager.ListeMonstres.Add(this);
            ID = Sc�neJeu.MonstManager.GetID();
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            UpdateMinionsID();
            V�rifier�tatMinions();
            V�rifier�tatH�ros();
            Visible = (Vector3.Distance(Position, Sc�neJeu.BaldorLeBrave.Position) < Cam�ra.DISTANCE_PLAN_�LOIGN�);
            if (PtsVie <= VieEntracte && Tampon)
            {
                Cr��rMinions();
                EstInvincible = true;
                Tampon = false;
                PtsAttaque *= 2;
                //Soundtrack.StartSoundCue(nomQuote) 
            }
            if (ToRemove)
            {
                Soundtrack.StartSoundCue("boss_death");
                Jeu.GestionSc�nes.ChangerDeSc�ne(Sc�nes.Credits);
            }
            if (HasAggroed && Soundtrack.IsSongCueStopped)
            {
                Soundtrack.StartSongCueWithDistanceEffect("boss_battle_loop", "Battle", Sc�neJeu.BaldorLeBrave.Position, Position, (AggroRange * 3));
            }
            else
            {
                if (HasAggroed)
                {
                    Soundtrack.UpdateSongCueDistanceEffect("Battle", Sc�neJeu.BaldorLeBrave.Position, Position, AggroRange * 3);
                }
            }
            base.Update(gameTime);
        }

        protected override void GestionCharm() { } //boss ne peut pas �tre charmed

        protected override void GestionAttaque()
        {
            if (Vector2.Distance(PositionCoord, Joueur.PositionCoord) < AggroRange)
            {
                D�buterAttaque(Joueur);
            }
        }

        private void Cr��rMinions()
        {
            Soundtrack.StartSoundCue("boss_minion");

            for (int i = 0; i < NB_MINIONS; i++)
            {
                int id = Sc�neJeu.MonstManager.GetID();
                ListeIDMinions.Add(id);
            }
            for (int i = 0; i < NB_MINIONS; ++i)
            {
                float angle = i * MathHelper.TwoPi / NB_MINIONS;
                Vector2 vDirection = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 positionminion = PositionCoord + vDirection * DiSTANCE_MINIONS;
                Sc�neJeu.MonstManager.Cr�erMonstre(Joueur, "Cyclops\\terrorwurm", 0.06f, Sc�neDeJeu.�CHELLE_BOX_WURM, new Vector3(positionminion.X, positionminion.Y, Position.Z),
                                                          new Vector3(0, 0, 0), new Vector3(0, -MathHelper.PiOver2, 0), "Minion", 15f, MathHelper.Pi * 4, false, 21, 3, 6, 4, 1f, false, 15, 80, 6, ListeIDMinions[0]);
            //"unicorn", 8f, 1f,
            }
        }
        public override void PerdrePointsDeVie(int D�gats)
        {
            if (!EstInvincible)
            {
                base.PerdrePointsDeVie(D�gats);
            }
        }
        private void V�rifier�tatMinions()
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
                if (!Sc�neJeu.MonstManager.IsIDActive(ListeIDMinions[i]))
                {
                    ListeIDMinions.RemoveAt(i);
                }
            }
        }

        protected override void V�rifier�tatH�ros()
        {
            if (Joueur.Reset)
            {
                PtsVie = PtsVieMax;
                PtsAttaque = PtsAttaqueInitial;
                Tampon = true;
                EstInvincible = false;
                foreach (int id in ListeIDMinions)
                {
                    Sc�neJeu.MonstManager.ListeMonstres.Remove(Sc�neJeu.MonstManager.GetMonsterWithID(id));
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
                Matrix view = Sc�neJeu.Cam�raJeu.Vue;

                Matrix projection = Sc�neJeu.Cam�raJeu.Projection;

                // Render the skinned mesh.
                foreach (ModelMesh mesh in Mod�le.Meshes)
                {
                    Matrix mondeLocal = TransformationsMod�le[mesh.ParentBone.Index] * GetMonde();
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
                foreach (ModelMesh maille in Mod�le.Meshes)
                {
                    Vector3[] oldColor = new Vector3[maille.MeshParts.Count];
                    int i = 0;
                    Matrix mondeLocal = TransformationsMod�le[maille.ParentBone.Index] * GetMonde();
                    foreach (ModelMeshPart portionDeMaillage in maille.MeshParts)
                    {
                        BasicEffect effet = (BasicEffect)portionDeMaillage.Effect;
                        oldColor[i] = effet.EmissiveColor;
                        effet.EnableDefaultLighting();
                        G�rerSurlignage(ref effet);
                        G�r�rCouleurCharm(ref effet);
                        effet.Projection = Sc�neJeu.Cam�raJeu.Projection;
                        effet.View = Sc�neJeu.Cam�raJeu.Vue;
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
                if (lineEffect == null) // Comme �a on a qu'� le cr�er une seule fois (et on ne peut le cr�er dans le Initialize())
                {
                    lineEffect = new BasicEffect(Sc�neJeu.GraphicsDevice);
                    lineEffect.LightingEnabled = false;
                    lineEffect.TextureEnabled = false;
                    lineEffect.VertexColorEnabled = true;
                }
                foreach (BoundingBoxBuffers bb in BoxDrawList)
                {
                    DrawBoundingBox(bb, lineEffect, Sc�neJeu.GraphicsDevice, Sc�neJeu.Cam�raJeu.Vue, Sc�neJeu.Cam�raJeu.Projection);
                }
            }
        }

        public override void D�buterAttaque(Combattant cible)
        {
            if (!HasAggroed)
            {
                HasAggroed = true;
                Soundtrack.StartSongCue("boss_battle_start");
            }
            base.D�buterAttaque(cible);
        }

        protected override void GestionUnleash()
        {
            if (HasAggroed)
            {
                HasAggroed = false;
                Soundtrack.StopSong();
                Sc�neJeu.MapManager.StartMusic();
            }
            base.GestionUnleash();
        }
    }
}