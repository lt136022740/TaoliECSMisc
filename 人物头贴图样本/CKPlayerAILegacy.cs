using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AICharacter
{
    Character1 = 1,
    Character2 = 2,
    Character3 = 3,
    Character4 = 4,
    Character5 = 5,
    Character6 = 6, //小坦克
}

public enum AIArmor
{
    None = 0,
    Armor1 = 22,
    Armor2 = 23,
    Armor3 = 24
}

public enum AIHelmet
{
    None = 0,
    Helmet1 = 19,
    Helmet2 = 20,
    Helmet3 = 21
}

public enum AIWeapon1
{
    //"M4A1", "Kar98k", "AKM", "Scar", "UMP", "RPG", "S1897",
    WeaponM4A1 = 12,
    WeaponKar98k = 13,
    WeaponAKM = 14,
    WeaponScar = 15,
    WeaponUMP = 16,
    WeaponRPG = 17,
    WeaponS1897 = 18,
    SmallArti = 25,
}

public class CKPlayerAILegacy : CKPlayerBase {
    private Transform m_kTransWeapon;
    public static CKGamePlayer instance;
    private CKPlayerBase m_kTransTarget;
    private NavMeshAgent m_kAgent;
    public Animator m_kAnimator;
    private Rigidbody m_kRigidbody;
    public CKPlayerAIType m_kAIType;
    //public Transform m_kPotrolPoint1;
    //public Transform m_kPotrolPoint2;
    public Transform m_kPotrolPointRoot;
    private Transform[] m_kPotrolPointArray;
    private Transform m_kNowPotrolDes;
    public AICharacter m_kEnumCharacter = AICharacter.Character1;
    public AIArmor m_kEnumArmor;
    public AIHelmet m_kEnumHelmet;
    public AIWeapon1 m_kEnumWeapon = AIWeapon1.WeaponUMP;
    private int[] dropId = new int[7];
    public Transform m_kTransBulletRoot;

    private void Start()
    {
        PlayerData data = new PlayerData();
        m_fRangeDis = GetFeelRange();
        m_fFireInterval = GetFireInterval();
        CF_Character.DataEntry dataConfig = DataConfigManager.Shared.csv_character.GetEntryPtr((int)m_kEnumCharacter);

        data.dataArmor = CreateData((int)m_kEnumArmor, 1);
        data.dataHelmet = CreateData((int)m_kEnumHelmet, 1); ;
        data.dataWeapon1 = CreateData((int)m_kEnumWeapon, 1);
        data.dataConfig = dataConfig;

        Init(data);
            
        dropId[0] = 3;
        dropId[1] = 10;
        dropId[2] = 12;
        dropId[3] = 15;
        dropId[4] = 16;
        dropId[5] = 17;
        dropId[6] = 18;

        int length = m_kPotrolPointRoot.childCount;
        m_kPotrolPointArray = new Transform[length];
        for (int i = 0; i < length; i++)
        {
            m_kPotrolPointArray[i] = m_kPotrolPointRoot.GetChild(i);
        }

    }

    public float GetFireInterval()
    {
        switch (m_kEnumCharacter)
        {
            case AICharacter.Character1:
                return 1.2f;
            case AICharacter.Character2:
                return 1;
            case AICharacter.Character3:
                return 0.8f;
            case AICharacter.Character4:
                return 0.6f;
            case AICharacter.Character5:
                return 0.4f;
            case AICharacter.Character6:
                return 0.2f;
        }
        return 1;
    }

    public float GetFeelRange()
    {
        switch (m_kEnumCharacter)
        {
            case AICharacter.Character1:
                return 12;
            case AICharacter.Character2:
                return 17;
            case AICharacter.Character3:
                return 22;
            case AICharacter.Character4:
                return 27;
            case AICharacter.Character5:
                return 32;
            case AICharacter.Character6:
                return 37;
        }
        return 10;
    }

    public ItemData CreateData(int itemID, int count)
    {
        if (itemID != 0)
        {
            CF_Item.DataEntry dataEntryArmorConfig = DataConfigManager.Shared.csv_item.GetEntryPtr(itemID);
            ItemData data = new ItemData();
            data.id = 0;
            data.m_fDurability = 100;
            data.m_iEquiped = 1;
            data.m_iItemID = dataEntryArmorConfig.ID;
            data.m_kCount = count;
            data.m_kIconPath = dataEntryArmorConfig.item_icon;
            data.m_kItemModel = dataEntryArmorConfig.item_model;
            data.m_kItemType = (ItemType)dataEntryArmorConfig.item_type;
            return data;
        }
        else
        {
            return null;
        }
    }

    public override void Init(PlayerData data)
    {
        base.Init(data);
        

        if(m_kEnumCharacter == AICharacter.Character1)
        {
            m_fCurrentHP = 10;
            m_fMaxHP = 10;
        }
        else if (m_kEnumCharacter == AICharacter.Character2)
        {
            m_fCurrentHP = 20;
            m_fMaxHP = 20;
        }
        else if (m_kEnumCharacter == AICharacter.Character3)
        {
            m_fCurrentHP = 34;
            m_fMaxHP = 34;
        }

        m_kTeam = GameTeam.TEAM_ENERMY;
        m_kRigidbody = GetComponentInChildren<Rigidbody>();
        m_kAnimator = GetComponentInChildren<Animator>();
        m_kCollider = GetComponentInChildren<CapsuleCollider>(  );
        m_kTransWeapon = EXGameObject.FindChild(transform, "WeaponNode");

        m_kAgent = GetComponent<NavMeshAgent>();
        //System.Action<Object> action = delegate (Object obj)
        //{
        //    GameObject kGo = GameObject.Instantiate(obj) as GameObject;
        //    kGo.transform.SetParent(m_kTransWeapon);
        //    kGo.transform.localPosition = Vector3.zero;
        //    kGo.transform.localScale = Vector3.one;
        //    kGo.transform.localRotation = Quaternion.identity;
        //    m_kWeapon = kGo.GetComponent<CKWeaponBase>();
        //    m_kWeapon.Init(m_kDataPlayer.dataWeapon1, this);
        //};
        //ABManager.Shared.LoadAssetBundleAsync(m_kDataPlayer.dataWeapon1.m_kItemModel, action);

        //System.Action<Object> actionTexture = delegate (Object go)
        //{
        //    Texture texture = go as Texture;
        //    for (int i = 0; i < m_kRenders.Length; ++i)
        //    {
        //        m_kRenders[i].material.SetTexture("_MainTex", texture);
        //        float shin = m_kDataPlayer.dataConfig.shin;
        //        float color = m_kDataPlayer.dataConfig.color / 255.0f;
        //        float spec = m_kDataPlayer.dataConfig.specular / 255.0f;
        //        m_kRenders[i].material.SetColor("_Color", new Color(color, color, color, 1));
        //        m_kRenders[i].material.SetColor("_SpecColor", new Color(spec, spec, spec, 1));
        //        m_kRenders[i].material.SetColor("_Shininess", new Color(shin, shin, shin, shin));
        //    }
        //};
        //ABManager.Shared.LoadAssetBundleAsync(m_kDataPlayer.dataConfig.texture_path, actionTexture);
        CKPlayerAIManager.Shared.Add(this);
    }
   

    public override void Die()
    {
        base.Die();
        CKPlayerAIManager.Shared.Delete(this);
        if (m_kAgent.enabled)
        {
            m_kAgent.isStopped = true;
        }
        //m_kAnimator.SetBool("Dead", true);
        m_kCollider.enabled = false;
        StopFire();
        m_kAnimation.Play("death_A");
        ReplaceMaterial();
        GenerateDropThing();
        CKBoot.Shared.m_kCurrentBattle.m_kNowDungeon.m_iEnermyCount--;
        UIManager.Shared.UI<UIMainBattle>(UIName.UIMainBattle).SetEnermyCount();
        GameObject.Destroy(m_kHPTitle.gameObject);
        //GameObject.Destroy(m_kWeapon.gameObject);

        m_kHPTitle = null;
    }

    public void ReplaceMaterial()
    {
        System.Action<Object> action = delegate (Object obj)
        {
            Material mat = obj as Material;

            System.Action<Object> action1 = delegate (Object obj1)
            {
                Texture texture = obj1 as Texture;
                mat.SetTexture("_MainTex", texture);
                mat.SetFloat("_DissElapsed", Time.realtimeSinceStartup);
                for(int i=0; i<m_kRenders.Length; ++i)
                {
                    m_kRenders[i].material = mat;
                }
            };
            ABManager.Shared.LoadAssetBundleAsync(m_kDataPlayer.dataConfig.texture_path, action1);
        };

        ABManager.Shared.LoadAssetBundleAsync("gfx/Dissolve/Materials/mat_dissolve", action);
    }

    public void GenerateDropThing()
    {
        int count = DataConfigManager.Shared.csv_item.m_kDataEntryTable.Count;
        int index = UnityEngine.Random.Range(0, dropId.Length);
        int itemID = dropId[index];
        CF_Item.DataEntry data = DataConfigManager.Shared.csv_item.GetEntryPtr(itemID);

        System.Action<Object> action1 = delegate (Object obj)
        {
            GameObject go = GameObject.Instantiate (obj) as GameObject;
            go.transform.position = transform.position;
            go.layer = LayerMask.NameToLayer("PickableObject");

            DropThing kComponent = go.GetComponent<DropThing>();
            ItemData itemData = new ItemData();
            itemData.m_iItemID = data.ID;
            itemData.m_kCount = 1;
            itemData.m_kIconPath = data.item_icon;
            itemData.m_kItemModel = data.item_model;
            itemData.m_kItemType = (ItemType)data.item_type;
            kComponent.m_kItemData = itemData;

            if (GameUtil.IsWeaponItem(itemID))
            {
                CF_Weapon.DataEntry dataConfig = (CF_Weapon.DataEntry)DataConfigManager.Shared.csv_weapon.m_kDataEntryTable[itemID];
                itemData.m_fDurability = dataConfig.attenuation;
            }

            System.Action<Object> action = delegate (Object obj1)
            {
                GameObject go1 = GameObject.Instantiate(obj1) as GameObject;
                go1.transform.SetParent(kComponent.m_kWeaponRoot);
                go1.transform.localScale = Vector3.one;
                go1.transform.localRotation = Quaternion.identity;
                go1.transform.localPosition = Vector3.zero;
            };
            ABManager.Shared.LoadAssetBundleAsync(data.item_model, action);
        };

        ABManager.Shared.LoadAssetBundleAsync("gfx/DropThing/DropThing", action1);
    }

    //public override void Fire()
    //{
    //    fire = true;
    //    m_kAnimation.Play( "combat_shoot_burst" );
    //    m_kWeapon.Fire();
    //    //StartCoroutine(AsyncFire());
    //}

    public override void Fire()
    {
        fire = true;
       
    }

    public bool fire = false;

    public override void StopFire()
    {
        fire = false;
        m_kAnimation.Play("combat_idle_aim");
        //m_kWeapon.StopFire();
        //m_kAnimator.SetBool("Aiming", false);
        //m_kAnimator.SetBool("Shoot", false);
    }

    public override void SetToIdle()
    {
        m_kAnimation.Play( "combat_idle_aim" );
        //m_kAnimator.SetBool("Armed", true);
        //m_kAnimator.SetBool("OnGround", false);
        //m_kAnimator.SetFloat("Speed", 0);
    }

    public override void SetToRun(float speed)
    {
        m_kAnimation.Play("combat_run");
        //m_kAnimator.SetFloat("Speed", speed);
        //m_kAnimator.SetBool("OnGround", true);
        //m_kAnimator.SetBool("Armed", true);
    }
    
    private float m_fElapsedDir;
    private Vector3 m_kLastPosition = Vector3.zero;
    public void SetDir(float deltaTime)
    {
        m_fElapsedDir += deltaTime;
        {
            m_fElapsedDir = 0;
            Vector3 diff = transform.position - m_kLastPosition;
            if (diff != Vector3.zero)
            {
                m_kLastPosition = transform.position;
                transform.rotation = Quaternion.LookRotation( diff );
            }

        }
    }

    public override void TakeDamge(float dmg)
    {
        base.TakeDamge(dmg);
        if(m_kHPTitle != null)
        {
            m_kHPTitle.SetHpPercent(m_fCurrentHP / m_fMaxHP);
        }
    }

    public void GenNextPotrolPoint()
    {
        if(m_kNowPotrolDes != null)
        {
            while (true)
            {
                var tempDes = m_kPotrolPointArray[UnityEngine.Random.Range(0, m_kPotrolPointArray.Length)];
                if(tempDes != m_kNowPotrolDes)
                {
                    m_kNowPotrolDes = tempDes;
                    return;
                }
            }
        }
        else
        {
            m_kNowPotrolDes = m_kPotrolPointArray[UnityEngine.Random.Range(0, m_kPotrolPointArray.Length)];
        }
    }

    private float m_fElapsedInRange;
    public void UpdateWhenInRange(float deltaTime)
    {
        m_fElapsedInRange += deltaTime;
        if(m_fElapsedInRange >= 0.4f)
        {
            transform.LookAt(m_kTransTarget.transform.position);
        }
    }

    bool m_bPatroling;
    bool m_bPausePatroling=true;
    float m_fPausePatrolingElasped;
    float m_fPausePatrolingInterval;
    private void UpdatePortral(float deltaTime)
    {
        if (m_kUnitState == EnermyState.Portral)
        {
            if (m_bPausePatroling)
            {
                m_fPausePatrolingElasped += Time.deltaTime;
                if (m_fPausePatrolingElasped >= m_fPausePatrolingInterval)
                {
                    SetToRun(0.3f);
                    GenNextPotrolPoint();
                    m_bPausePatroling = false;
                }
                return;
            }

            Vector3 pos1 = m_kNowPotrolDes.position;
            Vector3 pos2 = transform.position;

            if (!m_bPatroling)
            {
                m_bPatroling = true;
                m_kAgent.SetDestination(pos1);
            }
            else
            {
                pos1.y = 0;
                pos2.y = 0;

                float distance = Vector3.Distance(pos1, pos2);
                if (distance <= 0.2f)
                {
                    SetToIdle();
                    m_bPausePatroling = true;
                    m_fPausePatrolingElasped = 0;
                    m_fPausePatrolingInterval = Random.Range(2, 5);
                    m_bPatroling = false;
                }
            }
        }
    }


    private bool m_bChaseTarget = false;
    private float m_fChaseElapsed = 0;
    private float m_fChaseInterval = 0.5f;
    private float m_fChasedTime = 3;
    private float m_fChasedElapsed;

    private void UpdateChase(float deltaTime)
    {
        m_kTransTarget = CKPlayerManager.Shared.m_kMainPlayer;
        if (m_kTransTarget != null)
        {
            if (!m_kTransTarget.m_bDead)
            {
                if (m_bChaseTarget)
                {
                    Vector3 kThisPosition = transform.position;
                    Vector3 kThatPosition = m_kTransTarget.transform.position;

                    float distance = Vector3.Distance(kThisPosition, kThatPosition);
                            
                    m_fChasedElapsed += deltaTime;
                    if (m_fChasedElapsed >= m_fChasedTime)
                    {
                        m_bChaseTarget = false;
                        m_bStartFire = false;
                        //m_kWeapon.StopFire();
                        StopFire();
                        GenNextPotrolPoint();
                        m_kAgent.SetDestination(m_kNowPotrolDes.position);
                    }else
                    {
                        m_kAgent.SetDestination(kThatPosition);
                    }
                }
            }
        }
    }
    
    private bool m_bStartFire = false;
    private bool m_bHasTargetInRange = false;
    private float m_fTimeInterval = 0.5f;
    private bool m_bSearchEnd = false;
    private bool m_bNowSeaching = false;
    private float m_fElapsed;
    private void UpdateSearch(float deltaTime)
    {
        if (m_bSearchEnd)
        {
            return;
        }
        m_kTransTarget = CKPlayerManager.Shared.m_kMainPlayer;
        if (m_kTransTarget != null )
        {
            if (!m_kTransTarget.m_bDead)
            {
                m_fElapsed += deltaTime;
                if (m_fElapsed >= m_fTimeInterval)
                {
                    m_fElapsed = 0;
                    Vector3 kThisPosition = transform.position;
                    Vector3 kThatPosition = m_kTransTarget.transform.position;

                    float distance = Vector3.Distance(kThisPosition, kThatPosition);

                    bool bHasObstocleBetween = false;
                    if (distance <= m_fRangeDis-1)
                    {
                        RaycastHit hit;
                        Vector3 dir = kThatPosition - kThisPosition;
                        dir.y = 0;
                        bool hited = Physics.Raycast(kThisPosition, dir, out hit, distance);

                        if (hited)
                        {
                            if (!hit.collider.tag.Equals("Soldier"))
                            {
                                bHasObstocleBetween = true;
                            }
                        }
                    }

                    if (distance <= m_fRangeDis-1 && !bHasObstocleBetween)
                    {
                        m_bHasTargetInRange = true;
                        m_bChaseTarget = false;
                        transform.LookAt(kThatPosition);
                        if (!m_bStartFire)
                        {
                            m_bStartFire = true;
                            m_kAgent.isStopped = true;
                            Fire();
                        }
                    }
                    else if(!m_bChaseTarget && m_bHasTargetInRange)
                    {
                        m_bChaseTarget = true;
                        m_bHasTargetInRange = false;
                        m_bStartFire = false;
                        fire = false;
                        SetToRun(1);
                        m_kAgent.isStopped = false;
                        m_fChasedElapsed = 0;
                        StopFire();
                        //m_kWeapon.StopFire();
                    }
                }
            }
            else
            {
                m_bSearchEnd = true;
                StopFire();
                SetToIdle();
                m_kAgent.isStopped = true;
            }
        }
    }

   

    Vector3 dir = Vector3.zero;
    Vector3 dir1 = Vector3.zero;
    Vector3 dir2 = Vector3.zero;
    Vector3 kMainPlayerPosition = Vector3.zero;
    private float m_fHideElasped;
    private bool m_bReached = true;
    private Vector3 m_kNowHidePosition = Vector3.zero;

    public void UpdateHide(float deltaTime)
    {
        m_fHideElasped += deltaTime;
        kMainPlayerPosition = CKPlayerManager.Shared.m_kMainPlayer.transform.position;

        if (m_bReached)
        {
            if (m_fHideElasped >= 3)
            {
                m_fHideElasped = 0;
                float dis = int.MaxValue;
                Transform tran = null;
                for (int i = 0; i < CKBoot.Shared.m_kCurrentBattle.m_kTransCovers.Length; ++i)
                {
                    GameObject go = CKBoot.Shared.m_kCurrentBattle.m_kTransCovers[i];
                    Vector3 diff = transform.position - go.transform.position;
                    diff.y = 0;
                    float distance = diff.magnitude;
                    if (distance < dis)
                    {
                        dis = distance;
                        tran = go.transform;
                    }
                }

                Transform[] kTransTags = tran.GetComponentsInChildren<Transform>();

                Transform t1 = null;
                Transform t2 = null;
                float maxAngle = 0;

                for (int i = 0; i < kTransTags.Length; ++i)
                {
                    Vector3 tmp = kTransTags[i].position;
                    Vector3 dirOne = tmp - kMainPlayerPosition;
                    float radian = 0;
                    dirOne.y = 0;
                    dirOne.Normalize();

                    for (int j = 0; j < kTransTags.Length; ++j)
                    {
                        if (kTransTags[j] != kTransTags[i])
                        {
                            Vector3 mm = kTransTags[j].position - kMainPlayerPosition;
                            mm.y = 0;
                            mm.Normalize();

                            float d = Vector3.Dot(dirOne, mm);
                            float ad = Mathf.Acos(d);
                            if (ad > maxAngle)
                            {
                                maxAngle = ad;
                                radian = ad;
                                t1 = kTransTags[i];
                                t2 = kTransTags[j];
                            }
                        }
                    }
                }

                if(t1 == null || t2 == null){
                    return;
                }

                dir1 = t1.position - kMainPlayerPosition;
                dir2 = t2.position - kMainPlayerPosition;
                dir = Vector3.Lerp(dir1, dir2, 0.5f).normalized;

                RaycastHit hit;
                bool b = Physics.Raycast(kMainPlayerPosition, dir * 50, out hit);
                if (b)
                {
                    Vector3 hipoint = hit.point;
                    RaycastHit[] hits = Physics.RaycastAll(hipoint + dir * 20, -dir * 100);
                    for (int i = 0; i < hits.Length; ++i)
                    {
                        if (hits[i].collider.gameObject == hit.collider.gameObject)
                        {
                            m_kNowHidePosition = hits[i].point + dir * 0.5f;
                            m_bReached = false;
                            m_kAnimation.Play("combat_run");
                            m_kAgent.SetDestination(m_kNowHidePosition);
                        }
                    }
                }
            }
        }else
        {
            Vector3 diff = transform.position - m_kNowHidePosition;
            diff.y = 0;
            float distance = diff.magnitude;
            if(distance <= 0.9f)
            {
                m_kAnimation.Play("crouch_idle");
                m_bReached = true;
            }
        }
        
        Debug.DrawRay(kMainPlayerPosition, dir * 50, Color.red);
        Debug.DrawRay(kMainPlayerPosition, dir1 * 2);
        Debug.DrawRay(kMainPlayerPosition, dir2 * 2);
    }

    private float m_fDieElapsed;
    private void UpdateDie(float deltaTime)
    {
        m_fDieElapsed += deltaTime;
        if(m_fDieElapsed >= 10)
        {
            m_bDestoryed = true;
            GameObject.Destroy(gameObject);
        }
    }

    private float ttt;
    private float fixedTime = 6;
    float mm;
    private void UpdateRandomFire(float deltaTime)
    {
        //if (!m_kWeapon.m_bFire)
        //{
        //    ttt += deltaTime;

        //    if (ttt >= fixedTime)
        //    {
        //        fixedTime = Random.Range(6, 12);
        //        ttt = 0;
        //        if (!m_bHasTargetInRange)
        //        {
        //            m_kWeapon.Fire();

        //        }
        //    }
        //}
        //else
        //{
        //    mm += deltaTime;
        //    if(mm >= 0.8f)
        //    {
        //        mm = 0;
        //        if (!m_bHasTargetInRange)
        //        {
        //            m_kWeapon.StopFire();
        //        }
        //    }
        //}
    }

    private float elapsedFire = 1.5f;
    private int countPerFire = 3;

    private IEnumerator UpdateFire1(int countFire, float elapsedFire)
    {
        for (int i = 0; i < countFire; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        GenerateBullet();
        for (int i = 0; i < countFire; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        GenerateBullet();
        for (int i = 0; i < countFire; i++)
        {
            yield return new WaitForEndOfFrame();
        }
        GenerateBullet();
                
        yield return new WaitForSeconds(elapsedFire);
    }

    private void StartUpdateFire1()
    {
        StartCoroutine(UpdateFire1(GetCountPerFireByCharacterType(), GetFireIntervalByCharactorType()));
    }

    private void StopUpdateFire1()
    {
        StopCoroutine("UpdateFire1");
    }

    private int GetCountPerFireByCharacterType()
    {
        int count = 0;
        switch (m_kEnumCharacter)
        {
            case AICharacter.Character1:
                count = 3;
                break;
            case AICharacter.Character2:
                count = 5;
                break;
            case AICharacter.Character3:
                count = 5;
                break;
            case AICharacter.Character4:
                count = 5;
                break;
            case AICharacter.Character5:
                count = 5;
                break;
            case AICharacter.Character6:
                count = 5;
                break;
        }
        return count;
    }

    private float GetFireIntervalByCharactorType()
    {
        float val = 0;
        switch (m_kEnumCharacter)
        {
            case AICharacter.Character1:
                val = 1.5f;
                break;
            case AICharacter.Character2:
                val = 1.0f;
                break;
            case AICharacter.Character3:
                val = 0.5f;
                break;
            case AICharacter.Character4:
                val = 0.5f;
                break;
            case AICharacter.Character5:
                val = 0.5f;
                break;
            case AICharacter.Character6:
                val = 0.5f;
                break;
        }
        return val;
    }

    private void GenerateBullet()
    {
        var weaponConfig = DataConfigManager.Shared.csv_weapon.GetEntryPtr(m_kDataPlayer.dataWeapon1.m_iItemID);
        System.Action<Object> action = delegate (Object obj)
        {
            GameObject go = GameObject.Instantiate(obj) as GameObject;
            go.transform.SetParent(CKBoot.Shared.RootBullet);
            go.transform.position = m_kTransBulletRoot.position;
            Quaternion q = transform.rotation;
            Vector3 kEuler = q.eulerAngles;
            float rY = UnityEngine.Random.Range(-3, 3);
            float rX = UnityEngine.Random.Range(-3, 3);
            float rZ = UnityEngine.Random.Range(-3, 3);

            Quaternion newQ = Quaternion.Euler(rX, kEuler.y + rY, rZ);
            go.transform.rotation = newQ;

            CKBulletBase kBullet = go.GetComponent<CKBulletBase>();
            kBullet.Init(weaponConfig.hit_gfx, weaponConfig, m_kTeam);
            CKBulletManager.Shared.AddBullet(kBullet);

            AudioManager.Shared.PlaySoundEffect(weaponConfig.sound_open_fire);
        };

        ABManager.Shared.LoadAssetBundleAsync(weaponConfig.bullet_model, action);
    }


    float elapsed = 0;
    private void UpdateFire(float deltaTime)
    {
        elapsed += deltaTime;
        if(elapsed >= m_fFireInterval)
        {
            elapsed = 0;
            var weaponConfig = DataConfigManager.Shared.csv_weapon.GetEntryPtr(m_kDataPlayer.dataWeapon1.m_iItemID);
            System.Action<Object> action = delegate (Object obj)
            {
                GameObject go = GameObject.Instantiate(obj) as GameObject;
                go.transform.SetParent(CKBoot.Shared.RootBullet);
                go.transform.position = m_kTransBulletRoot.position;
                Quaternion q = transform.rotation;
                Vector3 kEuler = q.eulerAngles;
                float rY = UnityEngine.Random.Range(-3, 3);
                float rX = UnityEngine.Random.Range(-3, 3);
                float rZ = UnityEngine.Random.Range(-3, 3);

                Quaternion newQ = Quaternion.Euler(rX, kEuler.y + rY, rZ);
                go.transform.rotation = newQ;

                CKBulletBase kBullet = go.GetComponent<CKBulletBase>();
                kBullet.Init(weaponConfig.hit_gfx, weaponConfig, m_kTeam);
                CKBulletManager.Shared.AddBullet(kBullet);

                AudioManager.Shared.PlaySoundEffect(weaponConfig.sound_open_fire);
            };

            ABManager.Shared.LoadAssetBundleAsync(weaponConfig.bullet_model, action);
        }
        
    }
    }

    private float m_fDestoryElapsed = 0;
    private float m_fDestoryInterval = 4;
    public override void UpdateLogic(float deltaTime)
    {
        base.UpdateLogic(deltaTime);
        if (!m_bDead)
        {
            if (!m_bDestoryed)
            {
                if (!m_bDead)
                {
                    float percent = m_fCurrentHP / m_fMaxHP;
                    if (percent >= 0.4f)
                    {
                        UpdateSearch(deltaTime);
                        UpdateChase(deltaTime);
                        //if(!m_bChaseTarget)
                        {
                            UpdatePortral(deltaTime);
                        }
                        UpdateRandomFire(deltaTime);
                    }
                    else
                    {
                        UpdateHide(deltaTime);
                    }

                    if (m_kHPTitle != null)
                    {
                       Vector3 newPosition = transform.position;
                        newPosition.y += 3;
                        Vector3 screenPosition = Camera.main.WorldToScreenPoint(newPosition);
                        Vector3 newUIPosition = CKBoot.Shared.UICamera.ScreenToWorldPoint(screenPosition);
                        newUIPosition.z = 0;
                        m_kHPTitle.transform.position = newUIPosition;
                    }
                }
                else
                {
                    UpdateDie(deltaTime);
                }
            }
            /////

            SetDir(deltaTime);
            if (m_bHasTargetInRange)
            {
                UpdateWhenInRange(deltaTime);
            }
        }
        else
        {
            m_fDestoryElapsed += deltaTime;
            if (m_fDestoryElapsed >= m_fDestoryInterval)
            {
                m_bDestoryed = true;
            }
        }
        if (fire)
        {
            UpdateFire(deltaTime);
        }
    }
}
