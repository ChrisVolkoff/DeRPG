using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace AtelierXNA
{
   public class CaméraSubjective : Caméra
   {
      const float ACCÉLÉRATION = 0.001f;
      const float VITESSE_INITIALE_ROTATION = 0.05f;
      const float VITESSE_INITIALE_TRANSLATION = 0.05f;
      const float DELTA_LACET = MathHelper.Pi / 180; 
      const float DELTA_TANGAGE = MathHelper.Pi / 180; 
      const float DELTA_ROULIS = MathHelper.Pi / 180; 
 
      Vector3 Direction { get; set; }
      Vector3 Latéral { get; set; }
      float VitesseTranslation { get; set; }
      float VitesseRotation { get; set; }

      float IntervalleMAJ { get; set; }
      float TempsÉcouléDepuisMAJ { get; set; }
      InputManager InputMgr { get; set; }

      bool estEnZoom;
      bool EstEnZoom
      {
         get { return estEnZoom; }
         set
         {
            float ratioAffichage = Jeu.GraphicsDevice.Viewport.AspectRatio;
            estEnZoom = value;
            if (estEnZoom)
            {
               CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF / 2, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            }
            else
            {
               CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, ratioAffichage, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
            }
         }
      }

      public CaméraSubjective(Game jeu, InputManager inputMgr, Vector3 positionCaméra, Vector3 cible, Vector3 orientation, float intervalleMAJ)
         : base(jeu)
      {
         InputMgr = inputMgr;
         CréerVolumeDeVisualisation(OUVERTURE_OBJECTIF, DISTANCE_PLAN_RAPPROCHÉ, DISTANCE_PLAN_ÉLOIGNÉ);
         CréerPointDeVue(positionCaméra, cible, orientation);
         EstEnZoom = false;
      }

      public override void Initialize()
      {
         VitesseRotation = VITESSE_INITIALE_ROTATION;
         VitesseTranslation = VITESSE_INITIALE_TRANSLATION;
         TempsÉcouléDepuisMAJ = 0;
         base.Initialize();
      }

      protected override void CréerPointDeVue()
      {
         OrientationVerticale = Vector3.Normalize(OrientationVerticale);
         Direction = Vector3.Normalize(Direction);
         Vue = Matrix.CreateLookAt(Position, Position + Direction, OrientationVerticale);
         GénérerFrustum();
      }

      protected override void CréerPointDeVue(Vector3 position, Vector3 cible, Vector3 orientation)
      {
         Position = position;
         Direction = cible - Position;
         OrientationVerticale = orientation;
         CréerPointDeVue();
      }

      public override void Update(GameTime gameTime)
      {
         float TempsÉcoulé = (float)gameTime.ElapsedGameTime.TotalSeconds;
         TempsÉcouléDepuisMAJ += TempsÉcoulé;
         GestionClavier();
         if (TempsÉcouléDepuisMAJ >= IntervalleMAJ)
         {
            if (InputMgr.ÉtatClavier.IsKeyDown(Keys.LeftShift) || InputMgr.ÉtatClavier.IsKeyDown(Keys.RightShift))
            {
               GérerAccélération();
               GérerDéplacement();
               GérerRotation();
               CréerPointDeVue();
            }
            TempsÉcouléDepuisMAJ = 0;
         }
         base.Update(gameTime);
      }

      private int GérerTouche(Keys touche)
      {
         return InputMgr.ÉtatClavier.IsKeyDown(touche) ? 1 : 0;
      }

      private void GérerAccélération()
      {
         int valAccélération = GérerTouche(Keys.Subtract) - GérerTouche(Keys.Add);
         if (valAccélération != 0)
         {
            IntervalleMAJ += ACCÉLÉRATION * valAccélération;
         }
      }

      private void GérerDéplacement()
      {
         float déplacementDirection = (GérerTouche(Keys.W) - GérerTouche(Keys.S)) * VitesseTranslation;
         float déplacementLatéral = (GérerTouche(Keys.D) - GérerTouche(Keys.A)) * VitesseTranslation;

         Latéral = Vector3.Cross(Direction,OrientationVerticale);
         Latéral = Vector3.Normalize(Latéral);

         Position += Direction * déplacementDirection;       
         Position += Latéral * déplacementLatéral;
         CréerPointDeVue();
      }

      private void GérerRotation()
      {
         GérerLacet();
         GérerTangage();
         GérerRoulis();
      }

      private void GérerLacet()
      {
          float déplacementRotation = 0;
          if (InputMgr.ÉtatClavier.IsKeyDown(Keys.Left))
          {
              déplacementRotation = DELTA_LACET * VitesseRotation;
          }
          if (InputMgr.ÉtatClavier.IsKeyDown(Keys.Right))
          {
              déplacementRotation = DELTA_LACET * -VitesseRotation;
          }
          Matrix Rotation = Matrix.CreateFromAxisAngle(OrientationVerticale, déplacementRotation);
          Direction = Vector3.Transform(Direction, Rotation);
          Direction = Vector3.Normalize(Direction);
      }

      private void GérerTangage()
      {
          float déplacementRotation = 0;
          if (InputMgr.ÉtatClavier.IsKeyDown(Keys.Up))
          {
              déplacementRotation = DELTA_TANGAGE * VitesseRotation;
          }
          if (InputMgr.ÉtatClavier.IsKeyDown(Keys.Down))
          {
              déplacementRotation = DELTA_TANGAGE * -VitesseRotation;
          }
          Matrix Rotation = Matrix.CreateFromAxisAngle(Latéral, déplacementRotation);
          Direction = Vector3.Transform(Direction, Rotation);
          Direction = Vector3.Normalize(Direction);

          Latéral = Vector3.Cross(Direction, OrientationVerticale);
          Latéral = Vector3.Normalize(Latéral);

          Rotation = Matrix.CreateFromAxisAngle(Latéral, déplacementRotation);

          OrientationVerticale = Vector3.Transform(OrientationVerticale, Rotation);
          OrientationVerticale = Vector3.Normalize(OrientationVerticale);
      }

      private void GérerRoulis()
      {
          float déplacementRotation = 0;
          if (InputMgr.ÉtatClavier.IsKeyDown(Keys.PageUp))
          {
              déplacementRotation = DELTA_ROULIS * VitesseRotation;
          }
          if (InputMgr.ÉtatClavier.IsKeyDown(Keys.PageDown))
          {
              déplacementRotation = DELTA_ROULIS * -VitesseRotation;
          }
          Matrix Rotation = Matrix.CreateFromAxisAngle(Direction, déplacementRotation);
          OrientationVerticale = Vector3.Transform(OrientationVerticale, Rotation);
          OrientationVerticale = Vector3.Normalize(OrientationVerticale);
      }

      private void GestionClavier()
      {
         if (InputMgr.EstNouvelleTouche(Keys.Z))
         {
            EstEnZoom = !EstEnZoom;
         }
      }
   }
}
