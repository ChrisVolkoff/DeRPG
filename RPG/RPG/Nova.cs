using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AtelierXNA
{
   public class Nova : PlanTexturé //présentement seulement instanciable pour le héros
   {
      static Vector3 ROTATION_NOVA = new Vector3(-MathHelper.PiOver2, 0, 0);

      TimeSpan Durée { get; set; }
      float RayonFinal { get; set; }
      int Damage { set; get; }

      float TempsDepuisMAJ { get; set; }
      TimeSpan TempsÉcoulé { get; set; }

      public bool ToRemove { get; private set; }

      public List<Monstre> ListeDejaTouchés { get; private set; }
      BoundingSphere AoE { get; set; }

      public Nova(RPG jeu, ScèneDeJeu scèneJeu, Vector3 position, float rayon, TimeSpan durée, int damage, string nomtexture)
         : base(jeu, scèneJeu, position, ROTATION_NOVA, Vector2.One, Vector2.One, nomtexture)
      {
         Durée = durée;
         RayonFinal = rayon;
         Damage = damage;
         Monde = Matrix.Identity;
         Monde = Matrix.CreateScale(0);
         Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
         Monde *= Matrix.CreateTranslation(Position);
      }

      public override void Initialize()
      {
         TempsÉcoulé = new TimeSpan(0);
         TempsDepuisMAJ = 0;
         ListeDejaTouchés = new List<Monstre>();
         base.Initialize();
      }

      protected override void LoadContent()
      {
         base.LoadContent();
      }

      public override void Update(GameTime gameTime)
      {
         TempsÉcoulé += gameTime.ElapsedGameTime;
         TempsDepuisMAJ += (float)gameTime.ElapsedGameTime.TotalSeconds;
         if (TempsDepuisMAJ >= Jeu.INTERVALLE_MAJ)
         {
            AoE = new BoundingSphere(Position, RayonFinal * (float)Math.Min(TempsÉcoulé.TotalSeconds / Durée.TotalSeconds, 1));
            Monde = Matrix.CreateScale(AoE.Radius);
            //Monde = Matrix.Identity;
            Monde *= Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, Rotation.Z);
            Monde *= Matrix.CreateTranslation(Position);
            foreach (Monstre m in ScèneJeu.MonstManager.ListeMonstres)
            {
               foreach (BoundingBox box in m.BoxList)
               {
                  if (!(ListeDejaTouchés.Contains(m)) && AoE.Intersects(box))
                  {
                     ListeDejaTouchés.Add(m);
                     m.PerdrePointsDeVie(Damage);
                  }
               }
            }

            if (TempsÉcoulé >= Durée)
            {
               ToRemove = true;
            }
            TempsDepuisMAJ = 0;
         }

         base.Update(gameTime);
      }

      public override void Draw(GameTime gameTime)
      {
         base.Draw(gameTime);
      }
   }

   public class NovaManager : Microsoft.Xna.Framework.DrawableGameComponent
   {
      RPG Jeu { get; set; }
      ScèneDeJeu ScèneJeu { get; set; }
      public List<Nova> ListeDeNovas { get; private set; }


      public NovaManager(RPG jeu, ScèneDeJeu scenejeu)
         : base(jeu)
      {
         Jeu = jeu;
         ScèneJeu = scenejeu;
         ListeDeNovas = new List<Nova>();
      }

      public Nova AjouterNova(Vector3 position, float rayon, TimeSpan durée, int damage, string nomtexture)
      {
         Nova tempNova = new Nova(Jeu, ScèneJeu, position, rayon, durée, damage, nomtexture);
         ListeDeNovas.Add(tempNova);
         tempNova.Initialize();
         return tempNova;
      }


      public override void Initialize()
      {
         base.Initialize();
      }

      public override void Update(GameTime gameTime)
      {
         CheckForNovasToRemove();
         UpdateNovas(gameTime);

         base.Update(gameTime);
      }

      private void CheckForNovasToRemove()
      {
         for (int i = ListeDeNovas.Count - 1; i >= 0; --i)
         {
            if (ListeDeNovas[i].ToRemove)
            {
               ListeDeNovas.RemoveAt(i);
            }
         }
      }

      private void UpdateNovas(GameTime gameTime)
      {
         foreach (Nova nova in ListeDeNovas)
         {
            nova.Update(gameTime);
         }
      }

      public override void Draw(GameTime gameTime)
      {
         foreach (Nova nova in ListeDeNovas)
         {
            if (nova.Visible)
            {
               nova.Draw(gameTime);
            }
         }
         base.Draw(gameTime);
      }
   }
}
