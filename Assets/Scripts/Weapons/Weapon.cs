using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    [Header("Commons: ")]
    public WeaponStats statData;
    [SerializeField] private Transform m_shottingPiont;
    [SerializeField] private GameObject m_bulletsPrefab;
    [SerializeField] private GameObject m_muzzeleFlashPrefab;

    private float m_curentFr;
    private int m_currentBullets;
    private float m_curReloadTime;
    private bool m_isShoted;
    private bool m_isReloading;

    [Header("Event: ")]
    public UnityEvent OnShoot;
    public UnityEvent OnReload;
    public UnityEvent OnReloadDone;

    private void Start()
    {
        LoadStat();
    }

    private void LoadStat()
    {
        if (statData == null) return;
        statData.Load();
        m_curentFr = statData.firerate;
        m_curReloadTime = statData.reloadTime;
        m_currentBullets = statData.bullets;
    }
    private void Update()
    {
        ReduceFireRate();
        ReduceReloadTime();
    }

    private void ReduceFireRate()
    {
        if (!m_isShoted) { return; }
        m_curentFr -= Time.deltaTime;

        if (m_curentFr > 0) { return; }
        m_curentFr = statData.firerate;
        m_isShoted = false;
    }

    private void ReduceReloadTime()
    {
        if (!m_isReloading) { return; }
        m_curReloadTime -= Time.deltaTime;
        if(m_curReloadTime > 0) { return; }
        LoadStat();
        m_isReloading = false;
        OnReloadDone?.Invoke();
    }
    public void Shoot(Vector3 targetDirection)
    {
        if (m_isShoted || m_shottingPiont == null || m_currentBullets <= 0) return;
        if (m_muzzeleFlashPrefab)
        {
            var muzzleFlashClone = Instantiate(m_muzzeleFlashPrefab, m_shottingPiont.position, transform.rotation);
            muzzleFlashClone.transform.SetParent(m_shottingPiont);
        }
        if (m_bulletsPrefab)
        {
            var bulletClone = Instantiate(m_bulletsPrefab, m_shottingPiont.position, transform.rotation);
            var projecttileComp = bulletClone.GetComponent<ProjectTiles>();
            if (projecttileComp != null)
            {
                projecttileComp.Damaged = statData.damage;
            }
        }
        m_currentBullets--;
        m_isShoted = true;
        if (m_currentBullets <= 0)
        {
            Reload();
        }
        OnShoot?.Invoke();
    }
   
    public void Reload()
    {
        m_isReloading = true;
        OnReload?.Invoke();
    }
}
