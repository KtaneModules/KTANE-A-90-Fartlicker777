using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using KModkit;

using System.Reflection;

using Rnd = UnityEngine.Random;
using Math = ExMath;

public class A90 : MonoBehaviour {

   public KMBombInfo Bomb;
   public KMAudio Audio;
   public KMNeedyModule Needy;

   public AudioSource Warning;
   public AudioSource JumpscareLoudness;

   public Image A90Peaceful;
   public Image Jumpscare;
   public Image Static;
   public Image Stopsign;

   float MovementOpportunty = 27;
   int AILevel = 20;
   int FailedMovements = 0;

   float MouseXPos = 0;
   float MouseYPos = 0;

   Coroutine ObungulateStaticCor;

   bool Started;
   bool CheckForInputs;

   bool CurrentlyHeldL = false;
   bool CurrentlyHeldR = false;
   bool CurrentlyHeldM = false;
   bool MovedInput = false;

   bool DEBUGMODEACTIVE = false;
   bool DEBUGMODEALWAYSPASS = true;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   /*A90Settings Settings = new A90Settings();

   class A90Settings {
      public int AiLevel = 20;
      //bool JumpscaresEnabled = false;
   }

   static Dictionary<string, object>[] TweaksEditorSettings = new Dictionary<string, object>[]
   {
      new Dictionary<string, object>
      {
         { "Filename", "A90Settings.json" },
         { "Name", "A90 Settings" },
         { "Listing", new List<Dictionary<string, object>>{
            new Dictionary<string, object>
            {
               { "Key", "AI Level" },
               { "Text", "Affects how often A-90 attacks. Ranges from 1-50 (at a default of 20 AI), with higher AI levels making A-90 attack more often." },
            }
         } }
      }
   };*/

   

   
   
   void Awake () { //Avoid doing calculations in here regarding edgework. Just use this for setting up buttons for simplicity.




      ModuleId = ModuleIdCounter++;

      /*if (!Application.isEditor) {
         ModConfig<A90Settings> modConfig = new ModConfig<A90Settings>("A90");
         //Read from the settings file, or create one if one doesn't exist
         Settings = modConfig.Settings;
         //Update the settings file in case there was an error during read
         modConfig.Settings = Settings;
         AILevel = Settings.AiLevel;
      }

      string missionDesc = KTMissionGetter.Mission.Description;
      if (missionDesc != null) {
         Regex regex = new Regex(@"\^A90AI=$(true|false)");
         var match = regex.Match(missionDesc);
         if (match.Success) {
            string[] options = match.Value.Replace("A90=", "").Split(',');
            int value = 20;
            int.TryParse(options[0], out value);

            Settings.AiLevel = value;
         }
      }*/

      if (AILevel < 1) {
         AILevel = 1;
      }
      else if (AILevel > 50) {
         AILevel = 50;
      }

      //GetComponent<KMBombModule>().OnActivate += Activate;
      Needy.OnNeedyActivation += OnNeedyActivation;
      Needy.OnNeedyDeactivation += OnNeedyDeactivation;
      //Needy.OnTimerExpired += OnTimerExpired;
      /*
      foreach (KMSelectable object in keypad) {
          object.OnInteract += delegate () { keypadPress(object); return false; };
      }
      */

      //button.OnInteract += delegate () { buttonPress(); return false; };

   }

   void OnDestroy () { //Shit you need to do when the bomb ends
      
   }

   /*void Activate () { //Shit that should happen when the bomb arrives (factory)/Lights turn on

   }*/

   protected void OnNeedyActivation () { //Shit that happens when a needy turns on.
      if (!Started) {
         Started = true;
         StartCoroutine(ObungulateJumpscare());
         StartCoroutine(SetTime());
         StartCoroutine(Wait());
      }
   }

   IEnumerator ObungulateStatic () { //Only during attack should the thing obungulate.
      while (true) {
         Static.GetComponent<RectTransform>().anchoredPosition = new Vector2(Rnd.RandomRange(-100, 100), Rnd.RandomRange(-100, 100));
         yield return new WaitForSeconds(.05f);
      }
   }

   IEnumerator ObungulateJumpscare () {
      while (true) {
         Jumpscare.GetComponent<RectTransform>().anchoredPosition = new Vector2(Rnd.RandomRange(-10, 10), Rnd.RandomRange(-10, 10));
         yield return new WaitForSeconds(.025f);
      }
   }

   IEnumerator SetTime () {
      while (true) {
         Needy.SetNeedyTimeRemaining(90);
         yield return null;
      }
   }

   IEnumerator Wait () {
      while (true) {
         yield return new WaitForSeconds(DEBUGMODEACTIVE ? 7 : MovementOpportunty);
         if (Rnd.Range(1, 51) <= AILevel + FailedMovements * 2 || (DEBUGMODEACTIVE && DEBUGMODEALWAYSPASS)) {
            StartCoroutine(Attack());
         }
         else {
            FailedMovements++;
            if (Rnd.Range(0, 5) < 2) { //We don't want the fakeout on every failed movement as it would be annoying
               Audio.PlaySoundAtTransform("FakeoutA90", transform);
            }
         }
      }
   }

   IEnumerator Attack () {
      Warning.Play();
      A90Peaceful.GetComponent<RectTransform>().anchoredPosition = new Vector2(Rnd.RandomRange(-454, 454), Rnd.RandomRange(-144, 144));
      A90Peaceful.gameObject.SetActive(true);
      MovedInput = false;
      yield return new WaitForSeconds(.5f);
      MouseXPos = Input.mousePosition.x;
      MouseYPos = Input.mousePosition.y;
      CurrentlyHeldL = Input.GetMouseButton(0);
      CurrentlyHeldR = Input.GetMouseButton(1);
      CurrentlyHeldM = Input.GetMouseButton(2);
      CheckForInputs = true;
      A90Peaceful.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
      yield return new WaitForSeconds(.1f);
      Stopsign.gameObject.SetActive(true);
      Static.gameObject.SetActive(true);
      yield return new WaitForSeconds(.2f);
      Stopsign.gameObject.SetActive(false);
      Static.color = new Color32(92, 92, 92, 128);
      yield return new WaitForSeconds(.1f);
      Static.color = new Color32(255, 255, 255, 255);
      ObungulateStaticCor = StartCoroutine(ObungulateStatic());
      yield return new WaitForSeconds(.05f);
      A90Peaceful.color = new Color32(255, 0, 0, 255);
      yield return new WaitForSeconds(.05f);

      StopCoroutine(ObungulateStaticCor);
      A90Peaceful.gameObject.SetActive(false);
      Static.gameObject.SetActive(false);
      CheckForInputs = false;
      yield return new WaitForSeconds(.1f);

      A90Peaceful.color = new Color32(255, 255, 255, 255);
      Static.color = new Color32(92, 92, 92, 255);
      if (MovedInput) {
         StartCoroutine(Punish());
      }
      MovedInput = false;
   }

   IEnumerator Punish () {
      ObungulateStaticCor = StartCoroutine(ObungulateStatic());
      Static.gameObject.SetActive(true);
      Jumpscare.gameObject.SetActive(true);
      if (Rnd.Range(0, 500) == 0) {
         Audio.PlaySoundAtTransform("Dumbass", transform);
      }
      else {
         JumpscareLoudness.Play();
      }
      
      yield return new WaitForSeconds(1f); //Apparently it's at exactly 1 second.
      Static.gameObject.SetActive(false);
      Jumpscare.gameObject.SetActive(false);
      StopCoroutine(ObungulateStaticCor);
      Static.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
      Strike();
   }

   protected void OnNeedyDeactivation () { //Shit that happens when a needy turns off.
      Needy.OnPass();
   }

   void Start () { //Shit that you calculate, usually a majority if not all of the module
      Needy.SetResetDelayTime(30f, 50f);
      DEBUGMODEACTIVE = Application.isEditor;
      Debug.LogFormat("[A-90 #{0}] AI Level set at {1}", ModuleId, AILevel == 20 ? "20, which is the default." : (AILevel < 20 ? AILevel.ToString() + ", which is LOWER than the default. Cheating doodooheadass." : AILevel.ToString() + ", which is HIGHER than the default."));
   }

   void Update () { //Shit that happens at any point after initialization
      if (CheckForInputs && !MovedInput) {
         if ((Input.mousePosition.x != MouseXPos || Input.mousePosition.y != MouseYPos || CurrentlyHeldL != Input.GetMouseButton(0) || CurrentlyHeldR != Input.GetMouseButton(1) || CurrentlyHeldM != Input.GetMouseButton(2) || (Input.anyKey && CurrentlyHeldL != Input.GetMouseButton(0) && CurrentlyHeldR != Input.GetMouseButton(1) && CurrentlyHeldM != Input.GetMouseButton(2))) && !MovedInput) {
            MovedInput = true;
         }
      }
   }

   void Strike () {
      GetComponent<KMNeedyModule>().HandleStrike();
   }

#pragma warning disable 414
   private readonly string TwitchHelpMessage = @"Use !{0} to do something.";
#pragma warning restore 414

   IEnumerator ProcessTwitchCommand (string Command) {
      yield return null;
   }

   void TwitchHandleForcedSolve () { //Void so that autosolvers go to it first instead of potentially striking due to running out of time.
      StartCoroutine(HandleAutosolver());
   }

   IEnumerator HandleAutosolver () {
      yield return null;
   }
}
