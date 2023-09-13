// Decompiled with JetBrains decompiler
// Type: MissileLauncher
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: AD882E55-D8AC-4937-9773-540EE16F428F
// Assembly location: C:\Users\Marx_Admin\Documents\Daten\Github\Oni Mods\ONI-Ore_Maker\lib\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : 
  GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>
{
  private static StatusItem NoSurfaceSight = new StatusItem("MissileLauncher_NoSurfaceSight", "BUILDING", "status_item_no_sky", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
  private static StatusItem PartiallyBlockedStatus = new StatusItem("MissileLauncher_PartiallyBlocked", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID, false);
  public float shutdownDuration = 50f;
  public float shootDelayDuration = 0.25f;
  public static float SHELL_MASS = (float) ((double) MissileBasicConfig.recipe.ingredients[0].amount / 5.0 / 2.0);
  public static float SHELL_TEMPERATURE = 353.15f;
  public StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.BoolParameter rotationComplete;
  public StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.ObjectParameter<GameObject> meteorTarget = new StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.ObjectParameter<GameObject>();
  public StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.TargetParameter cannonTarget;
  public StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.BoolParameter fullyBlocked;
  public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State Off;
  public MissileLauncher.OnState On;
  public MissileLauncher.LaunchState Launch;
  public MissileLauncher.CooldownState Cooldown;
  public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State Nosurfacesight;
  public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State NoAmmo;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.Off;
    this.root.Update((System.Action<MissileLauncher.Instance, float>) ((smi, dt) => smi.HasLineOfSight()));
    this.Off.PlayAnim("inoperational").EventTransition(GameHashes.OperationalChanged, (GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State) this.On, (StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.Transition.ConditionCallback) (smi => smi.Operational.IsOperational)).Enter((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State.Callback) (smi => smi.Operational.SetActive(false)));
    this.On.DefaultState(this.On.opening).EventTransition(GameHashes.OperationalChanged, this.On.shutdown, (StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.Transition.ConditionCallback) (smi => !smi.Operational.IsOperational)).ParamTransition<bool>((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.Parameter<bool>) this.fullyBlocked, this.Nosurfacesight, GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.IsTrue).ScheduleGoTo(this.shutdownDuration, (StateMachine.BaseState) this.On.idle).Enter((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State.Callback) (smi => smi.Operational.SetActive(smi.Operational.IsOperational)));
    this.On.opening.PlayAnim("working_pre").OnAnimQueueComplete(this.On.searching).Target(this.cannonTarget).PlayAnim("Cannon_working_pre");
    this.On.searching.PlayAnim("on", KAnim.PlayMode.Loop).Enter((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State.Callback) (smi =>
    {
      smi.sm.rotationComplete.Set(false, smi);
      smi.sm.meteorTarget.Set((GameObject) null, smi);
      smi.cannonRotation = smi.def.scanningAngle;
    })).Update("FindMeteor", (System.Action<MissileLauncher.Instance, float>) ((smi, dt) => smi.Searching(dt)), UpdateRate.SIM_EVERY_TICK).EventTransition(GameHashes.OnStorageChange, this.NoAmmo, (StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.Transition.ConditionCallback) (smi => smi.MissileStorage.Count <= 0)).ParamTransition<GameObject>((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.Parameter<GameObject>) this.meteorTarget, this.Launch.targeting, (StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.Parameter<GameObject>.Callback) ((smi, meteor) => (UnityEngine.Object) meteor != (UnityEngine.Object) null)).Exit((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State.Callback) (smi => smi.sm.rotationComplete.Set(false, smi)));
    this.On.idle.Target(this.masterTarget).PlayAnim("idle", KAnim.PlayMode.Loop).UpdateTransition((GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State) this.On, (Func<MissileLauncher.Instance, float, bool>) ((smi, dt) => smi.Operational.IsOperational && smi.MeteorDetected())).Target(this.cannonTarget).PlayAnim("Cannon_working_pst");
    this.On.shutdown.Target(this.masterTarget).PlayAnim("working_pst").OnAnimQueueComplete(this.Off).Target(this.cannonTarget).PlayAnim("Cannon_working_pst");
    this.Launch.PlayAnim("target_detected", KAnim.PlayMode.Loop).Update("Rotate", (System.Action<MissileLauncher.Instance, float>) ((smi, dt) => smi.RotateToMeteor(dt)), UpdateRate.SIM_EVERY_TICK);
    this.Launch.targeting.Update("Targeting", (System.Action<MissileLauncher.Instance, float>) ((smi, dt) =>
    {
      if (((object) smi.sm.meteorTarget.Get(smi)).IsNullOrDestroyed())
      {
        smi.GoTo((StateMachine.BaseState) this.On.searching);
      }
      else
      {
        if ((double) smi.cannonAnimController.Rotation >= (double) smi.def.maxAngle * -1.0 && (double) smi.cannonAnimController.Rotation <= (double) smi.def.maxAngle)
          return;
        smi.sm.meteorTarget.Get(smi).GetComponent<Comet>().Targeted = false;
        smi.sm.meteorTarget.Set((GameObject) null, smi);
        smi.GoTo((StateMachine.BaseState) this.On.searching);
      }
    }), UpdateRate.SIM_EVERY_TICK).ParamTransition<bool>((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.Parameter<bool>) this.rotationComplete, this.Launch.shoot, GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.IsTrue);
    this.Launch.shoot.ScheduleGoTo(this.shootDelayDuration, (StateMachine.BaseState) this.Launch.pst).Exit("LaunchMissile", (StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State.Callback) (smi =>
    {
      smi.LaunchMissile();
      this.cannonTarget.Get(smi).GetComponent<KBatchedAnimController>().Play((HashedString) "Cannon_shooting_pre");
    }));
    this.Launch.pst.Target(this.masterTarget).Enter((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State.Callback) (smi =>
    {
      smi.SetOreChunk();
      KAnimControllerBase component = smi.GetComponent<KAnimControllerBase>();
      if (smi.GetComponent<Storage>().Count <= 0)
        component.Play((HashedString) "base_shooting_pst_last");
      else
        component.Play((HashedString) "base_shooting_pst");
    })).Target(this.cannonTarget).PlayAnim("Cannon_shooting_pst").OnAnimQueueComplete((GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State) this.Cooldown);
    this.Cooldown.Update("Rotate", (System.Action<MissileLauncher.Instance, float>) ((smi, dt) => smi.RotateToMeteor(dt)), UpdateRate.SIM_EVERY_TICK, false).Exit((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State.Callback) (smi => smi.SpawnOre())).Enter((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State.Callback) (smi =>
    {
      KAnimControllerBase component = smi.GetComponent<KAnimControllerBase>();
      if (smi.GetComponent<Storage>().Count <= 0)
        component.Play((HashedString) "base_ejecting_last");
      else
        component.Play((HashedString) "base_ejecting");
      smi.sm.rotationComplete.Set(false, smi);
      smi.sm.meteorTarget.Set((GameObject) null, smi);
    })).OnAnimQueueComplete(this.On.searching);
    this.Nosurfacesight.Target(this.masterTarget).PlayAnim("working_pst").QueueAnim("error").ParamTransition<bool>((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.Parameter<bool>) this.fullyBlocked, (GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State) this.On, GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.IsFalse).Target(this.cannonTarget).PlayAnim("Cannon_working_pst").Enter((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State.Callback) (smi => smi.Operational.SetActive(false)));
    this.NoAmmo.PlayAnim("off_open").EventTransition(GameHashes.OnStorageChange, (GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State) this.On, (StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.Transition.ConditionCallback) (smi => smi.MissileStorage.Count > 0)).Enter((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State.Callback) (smi => smi.Operational.SetActive(false))).Exit((StateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State.Callback) (smi => smi.GetComponent<KAnimControllerBase>().Play((HashedString) "off_closing"))).Target(this.cannonTarget).PlayAnim("Cannon_working_pst");
  }

  public class Def : StateMachine.BaseDef
  {
    public static readonly CellOffset LaunchOffset = new CellOffset(0, 4);
    public float launchSpeed = 30f;
    public float rotationSpeed = 100f;
    public static readonly Vector2I launchRange = new Vector2I(16, 32);
    public float scanningAngle = 50f;
    public float maxAngle = 80f;
  }

  public new class Instance : 
    GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.GameInstance
  {
    [MyCmpReq]
    public Operational Operational;
    [MyCmpReq]
    public Storage MissileStorage;
    [MyCmpReq]
    public KSelectable Selectable;
    [MyCmpReq]
    public FlatTagFilterable TargetFilter;
    private Vector3 launchPosition;
    private Vector2I launchXY;
    private float launchAnimTime;
    public KBatchedAnimController cannonAnimController;
    public GameObject cannonGameObject;
    public float cannonRotation;
    public float simpleAngle;
    private Tag missileElement;
    private MeterController meter;
    private WorldContainer worldContainer;

    public WorldContainer myWorld
    {
      get
      {
        if ((UnityEngine.Object) this.worldContainer == (UnityEngine.Object) null)
          this.worldContainer = this.GetMyWorld();
        return this.worldContainer;
      }
    }

    public Instance(IStateMachineTarget master, MissileLauncher.Def def)
      : base(master, def)
    {
      KBatchedAnimController component1 = this.GetComponent<KBatchedAnimController>();
      string name = component1.name + ".cannon";
      this.smi.cannonGameObject = new GameObject(name);
      this.smi.cannonGameObject.SetActive(false);
      this.smi.cannonGameObject.transform.parent = component1.transform;
      this.smi.cannonGameObject.AddComponent<KPrefabID>().PrefabTag = new Tag(name);
      this.smi.cannonAnimController = this.smi.cannonGameObject.AddComponent<KBatchedAnimController>();
      this.smi.cannonAnimController.AnimFiles = new KAnimFile[1]
      {
        component1.AnimFiles[0]
      };
      this.smi.cannonAnimController.initialAnim = "Cannon_off";
      this.smi.cannonAnimController.isMovable = true;
      this.smi.cannonAnimController.SetSceneLayer(Grid.SceneLayer.Building);
      component1.SetSymbolVisiblity((KAnimHashedString) "cannon_target", false);
      Vector3 column = (Vector3) component1.GetSymbolTransform(new HashedString("cannon_target"), out bool _).GetColumn(3) with
      {
        z = Grid.GetLayerZ(Grid.SceneLayer.Building)
      };
      this.smi.cannonGameObject.transform.SetPosition(column);
      this.launchPosition = column;
      Grid.PosToXY(this.launchPosition, out this.launchXY);
      this.smi.cannonGameObject.SetActive(true);
      this.smi.sm.cannonTarget.Set(this.smi.cannonGameObject, this.smi, false);
      KAnim.Anim anim = component1.AnimFiles[0].GetData().GetAnim("Cannon_shooting_pre");
      if (anim != null)
      {
        this.launchAnimTime = anim.totalTime / 2f;
      }
      else
      {
        Debug.LogWarning((object) "MissileLauncher anim data is missing");
        this.launchAnimTime = 1f;
      }
      this.meter = new MeterController((KAnimControllerBase) this.GetComponent<KBatchedAnimController>(), "meter_target", nameof (meter), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
      this.Subscribe(-1201923725, new System.Action<object>(this.OnHighlight));
      this.MissileStorage.Subscribe(-1697596308, new System.Action<object>(this.OnStorage));
      FlatTagFilterable component2 = this.smi.master.GetComponent<FlatTagFilterable>();
      foreach (GameObject go in Assets.GetPrefabsWithTag(GameTags.Comet))
      {
        if (!go.HasTag(GameTags.DeprecatedContent))
        {
          if (!component2.tagOptions.Contains(go.PrefabID()))
          {
            component2.tagOptions.Add(go.PrefabID());
            component2.selectedTags.Add(go.PrefabID());
          }
          component2.selectedTags.Remove((Tag) GassyMooCometConfig.ID);
        }
      }
    }

    public override void StartSM()
    {
      base.StartSM();
      this.OnStorage((object) null);
    }

    protected override void OnCleanUp()
    {
      this.Unsubscribe(-1201923725, new System.Action<object>(this.OnHighlight));
      base.OnCleanUp();
    }

    private void OnHighlight(object data) => this.smi.cannonAnimController.HighlightColour = this.GetComponent<KBatchedAnimController>().HighlightColour;

    private void OnStorage(object data) => this.meter.SetPositionPercent(Mathf.Clamp01(this.MissileStorage.MassStored() / this.MissileStorage.capacityKg));

    public void Searching(float dt)
    {
      this.FindMeteor();
      this.RotateCannon(dt, this.def.rotationSpeed / 2f);
      if (!this.smi.sm.rotationComplete.Get(this.smi))
        return;
      this.cannonRotation *= -1f;
      this.smi.sm.rotationComplete.Set(false, this.smi);
    }

    public void FindMeteor()
    {
      GameObject gameObject = this.ChooseClosestInterceptionPoint(this.myWorld.id);
      if (!((UnityEngine.Object) gameObject != (UnityEngine.Object) null))
        return;
      this.smi.sm.meteorTarget.Set(gameObject, this.smi);
      gameObject.GetComponent<Comet>().Targeted = true;
      this.smi.cannonRotation = this.CalculateLaunchAngle(gameObject.transform.position);
    }

    private float CalculateLaunchAngle(Vector3 targetPosition) => MathUtil.AngleSigned(Vector3.up, Vector3.Normalize(targetPosition - this.launchPosition), Vector3.forward);

    public void LaunchMissile()
    {
      GameObject first = this.MissileStorage.FindFirst((Tag) "MissileBasic");
      if (!((UnityEngine.Object) first != (UnityEngine.Object) null))
        return;
      Pickupable pickupable = first.GetComponent<Pickupable>();
      if ((double) pickupable.TotalAmount <= 1.0)
        this.MissileStorage.Drop(pickupable.gameObject, true);
      else
        pickupable = EntitySplitter.Split(pickupable, 1f);
      this.SetMissileElement(first);
      GameObject gameObject = this.smi.sm.meteorTarget.Get(this.smi);
      if (((object) gameObject).IsNullOrDestroyed())
        return;
      pickupable.GetSMI<MissileProjectile.StatesInstance>().PrepareLaunch(gameObject.GetComponent<Comet>(), this.def.launchSpeed, this.launchPosition, this.smi.cannonRotation);
    }

    private void SetMissileElement(GameObject missile)
    {
      this.missileElement = missile.GetComponent<PrimaryElement>().Element.tag;
      if (!((UnityEngine.Object) Assets.GetPrefab(this.missileElement) == (UnityEngine.Object) null))
        return;
      Debug.LogWarning((object) string.Format("Missing element {0} for missile launcher. Defaulting to IronOre", (object) this.missileElement));
      this.missileElement = GameTags.IronOre;
    }

    public GameObject ChooseClosestInterceptionPoint(int world_id)
    {
      GameObject gameObject = (GameObject) null;
      List<Comet> items = Components.Meteors.GetItems(world_id);
      float num1 = (float) MissileLauncher.Def.launchRange.y;
      foreach (Comet comet in items)
      {
        if (!((object) comet).IsNullOrDestroyed() && !comet.Targeted && this.TargetFilter.selectedTags.Contains(comet.typeID))
        {
          Vector3 targetPosition = comet.TargetPosition;
          float timeToCollision;
          Vector3 collisionPoint = this.CalculateCollisionPoint(targetPosition, (Vector3) comet.Velocity, out timeToCollision);
          Grid.PosToCell(collisionPoint);
          float num2 = Vector3.Distance(collisionPoint, this.launchPosition);
          if ((double) num2 < (double) num1 && (double) timeToCollision > (double) this.launchAnimTime && this.IsMeteorInRange(collisionPoint) && this.IsPathClear(this.launchPosition, targetPosition))
          {
            gameObject = comet.gameObject;
            num1 = num2;
          }
        }
      }
      return gameObject;
    }

    private bool IsMeteorInRange(Vector3 interception_point)
    {
      Vector2I xy;
      Grid.PosToXY(interception_point, out xy);
      return Math.Abs(xy.X - this.launchXY.X) <= MissileLauncher.Def.launchRange.X && xy.Y - this.launchXY.Y > 0 && xy.Y - this.launchXY.Y <= MissileLauncher.Def.launchRange.Y;
    }

    public bool IsPathClear(Vector3 startPoint, Vector3 endPoint)
    {
      Vector2I xy1 = Grid.PosToXY(startPoint);
      Vector2I xy2 = Grid.PosToXY(endPoint);
      return Grid.TestLineOfSight(xy1.x, xy1.y, xy2.x, xy2.y, new Func<int, bool>(this.IsCellBlockedFromSky), allow_invalid_cells: true);
    }

    public bool IsCellBlockedFromSky(int cell)
    {
      if (Grid.IsValidCell(cell) && (int) Grid.WorldIdx[cell] == this.myWorld.id)
        return Grid.Solid[cell];
      int y;
      Grid.CellToXY(cell, out int _, out y);
      return y <= this.launchXY.Y;
    }

    public Vector3 CalculateCollisionPoint(
      Vector3 targetPosition,
      Vector3 targetVelocity,
      out float timeToCollision)
    {
      Vector3 vector3 = targetVelocity - this.smi.def.launchSpeed * (targetPosition - this.launchPosition).normalized;
      timeToCollision = (targetPosition - this.launchPosition).magnitude / vector3.magnitude;
      return targetPosition + targetVelocity * timeToCollision;
    }

    public void HasLineOfSight()
    {
      bool flag = false;
      bool on = true;
      Extents extents = this.GetComponent<Building>().GetExtents();
      int val2_1 = this.launchXY.x - MissileLauncher.Def.launchRange.X;
      int val2_2 = this.launchXY.x + MissileLauncher.Def.launchRange.X;
      int y = extents.y + extents.height;
      int cell1 = Grid.XYToCell(Math.Max((int) this.myWorld.minimumBounds.x, val2_1), y);
      int cell2 = Grid.XYToCell(Math.Min((int) this.myWorld.maximumBounds.x, val2_2), y);
      for (int i = cell1; i <= cell2; ++i)
      {
        flag = flag || Grid.ExposedToSunlight[i] <= (byte) 0;
        on = on && Grid.ExposedToSunlight[i] <= (byte) 0;
      }
      this.Selectable.ToggleStatusItem(MissileLauncher.PartiallyBlockedStatus, flag && !on);
      this.Selectable.ToggleStatusItem(MissileLauncher.NoSurfaceSight, on);
      this.smi.sm.fullyBlocked.Set(on, this.smi);
    }

    public bool MeteorDetected() => Components.Meteors.GetItems(this.myWorld.id).Count > 0;

    public void SetOreChunk()
    {
      KAnim.Build.Symbol symbolByIndex = Assets.GetPrefab(this.missileElement).GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbolByIndex(0U);
      this.gameObject.GetComponent<SymbolOverrideController>().AddSymbolOverride((HashedString) "Shell", symbolByIndex);
    }

    public void SpawnOre()
    {
      Vector3 column = (Vector3) this.GetComponent<KBatchedAnimController>().GetSymbolTransform((HashedString) "Shell", out bool _).GetColumn(3) with
      {
        z = Grid.GetLayerZ(Grid.SceneLayer.Ore)
      };
      Assets.GetPrefab(this.missileElement).GetComponent<PrimaryElement>().Element.substance.SpawnResource(column, MissileLauncher.SHELL_MASS, MissileLauncher.SHELL_TEMPERATURE, byte.MaxValue, 0);
    }

    public void RotateCannon(float dt, float rotation_speed)
    {
      float num1 = this.cannonRotation - this.simpleAngle;
      if ((double) num1 > 180.0)
        num1 -= 360f;
      else if ((double) num1 < -180.0)
        num1 += 360f;
      float num2 = rotation_speed * dt;
      if ((double) num1 > 0.0 && (double) num2 < (double) num1)
      {
        this.simpleAngle += num2;
        this.cannonAnimController.Rotation = this.simpleAngle;
      }
      else if ((double) num1 < 0.0 && -(double) num2 > (double) num1)
      {
        this.simpleAngle -= num2;
        this.cannonAnimController.Rotation = this.simpleAngle;
      }
      else
      {
        this.simpleAngle = this.cannonRotation;
        this.cannonAnimController.Rotation = this.simpleAngle;
        this.smi.sm.rotationComplete.Set(true, this.smi);
      }
    }

    public void RotateToMeteor(float dt)
    {
      GameObject gameObject = this.sm.meteorTarget.Get(this);
      if (((object) gameObject).IsNullOrDestroyed())
        return;
      float num1 = this.CalculateLaunchAngle(gameObject.transform.position) - this.simpleAngle;
      if ((double) num1 > 180.0)
        num1 -= 360f;
      else if ((double) num1 < -180.0)
        num1 += 360f;
      float num2 = this.def.rotationSpeed * dt;
      if ((double) num1 > 0.0 && (double) num2 < (double) num1)
      {
        this.simpleAngle += num2;
        this.cannonAnimController.Rotation = this.simpleAngle;
      }
      else if ((double) num1 < 0.0 && -(double) num2 > (double) num1)
      {
        this.simpleAngle -= num2;
        this.cannonAnimController.Rotation = this.simpleAngle;
      }
      else
        this.smi.sm.rotationComplete.Set(true, this.smi);
    }
  }

  public class OnState : 
    GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State
  {
    public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State searching;
    public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State opening;
    public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State shutdown;
    public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State idle;
  }

  public class LaunchState : 
    GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State
  {
    public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State targeting;
    public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State shoot;
    public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State pst;
  }

  public class CooldownState : 
    GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State
  {
    public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State cooling;
    public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State exit;
    public GameStateMachine<MissileLauncher, MissileLauncher.Instance, IStateMachineTarget, MissileLauncher.Def>.State exitNoAmmo;
  }
}
