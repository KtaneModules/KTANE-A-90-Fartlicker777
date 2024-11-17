using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using KModkit;
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
   bool DEBUGMODEACTIVE = false;

   static int ModuleIdCounter = 1;
   int ModuleId;
   private bool ModuleSolved;

   void Awake () { //Avoid doing calculations in here regarding edgework. Just use this for setting up buttons for simplicity.
      ModuleId = ModuleIdCounter++;
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
         if (Rnd.Range(1, 51) <= AILevel + FailedMovements * 2 || DEBUGMODEACTIVE) {
            StartCoroutine(Attack());
         }
         else {
            FailedMovements++;
         }
      }
   }

   IEnumerator Attack () {
      Warning.Play();
      A90Peaceful.GetComponent<RectTransform>().anchoredPosition = new Vector2(Rnd.RandomRange(-454, 454), Rnd.RandomRange(-144, 144));
      A90Peaceful.gameObject.SetActive(true);
      yield return new WaitForSeconds(.5f);
      A90Peaceful.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
      Stopsign.gameObject.SetActive(true);
      Static.gameObject.SetActive(true);
      MouseXPos = Input.mousePosition.x;
      MouseYPos = Input.mousePosition.y;
      bool CurrentlyHeldL = Input.GetMouseButton(0);
      bool CurrentlyHeldR = Input.GetMouseButton(1);
      bool CurrentlyHeldM = Input.GetMouseButton(2);
      bool Attacked = false;
      while (Warning.isPlaying) {
         if ((Input.mousePosition.x != MouseXPos || Input.mousePosition.y != MouseYPos || CurrentlyHeldL != Input.GetMouseButton(0) || CurrentlyHeldR != Input.GetMouseButton(1) || CurrentlyHeldM != Input.GetMouseButton(2) || (Input.anyKey && CurrentlyHeldL != Input.GetMouseButton(0) && CurrentlyHeldR != Input.GetMouseButton(1) && CurrentlyHeldM != Input.GetMouseButton(2))) && !Attacked) {
            Attacked = true;
            //Debug.Log(CurrentlyHeldL);
            //Debug.Log(Input.GetMouseButton(0));
         }
         yield return null;
      }
      Stopsign.gameObject.SetActive(false);
      A90Peaceful.gameObject.SetActive(false);
      Static.gameObject.SetActive(false);
      yield return new WaitForSeconds(.1f);
      if (Attacked) {
         StartCoroutine(Punish());
      }
   }

   IEnumerator Punish () {
      ObungulateStaticCor = StartCoroutine(ObungulateStatic());
      Static.gameObject.SetActive(true);
      Jumpscare.gameObject.SetActive(true);
      JumpscareLoudness.Play();
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
   }

   void Update () { //Shit that happens at any point after initialization
      //Debug.LogFormat(Input.mousePosition.x.ToString());
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
