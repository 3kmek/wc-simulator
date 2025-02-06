using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridSystem : MonoBehaviour
{
    private static GridSystem _instance;
    public static GridSystem Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GridSystem>();
                if (_instance == null)
                {
                    Debug.LogError("GridSystem bulunamadı! Sahneye eklediniz mi?");
                }
            }
            return _instance;
        }
    }

    [Header("Grid Settings")]
    public float gridSize = 5f;
    [SerializeField] private float removeRange = 10f;

    // Eğer ghostPrefab üzerinde özel bir texture/shader kullanmak isterseniz bu Textur’u render materialına atayabilirsiniz.
    [Header("Optional Ghost Texture")]
    [SerializeField] private Texture ghostObjectTexture;

    private GameObject ghostObject; // Şu an sahnede olan hayalet obje
    private GameObject realPrefab;  // Yerleştirme anında Instantiate edeceğimiz gerçek obje

    // Yerleştirilmiş konumlar
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    private void Update()
    {
        // Build modu kapalıysa ghost'u devre dışı bırak
        if (!BuildManager.Instance.isBuildingMode)
        {
            if (ghostObject != null)
                ghostObject.SetActive(false);

            return;
        }

        // Build modu açık ve ghost mevcutsa görünür yap
        if (ghostObject != null && !ghostObject.activeSelf)
            ghostObject.SetActive(true);

        UpdateGhostPositions();

        if (CanPlaceObject())
            SetGhostColor(new Color(1f, 1f, 1f, 0.5f));
        else
            SetGhostColor(Color.red);
    }

    /// <summary>
    /// BuildManager'dan çağrılır; yeni bir realPrefab + ghostPrefab set'i atanır.
    /// </summary>
    public void SetObjectToPlace(GameObject newRealPrefab, GameObject newGhostPrefab)
    {
        // Önce eski ghost varsa yok edelim
        if (ghostObject != null)
            Destroy(ghostObject);

        // Referansları saklayalım
        realPrefab = newRealPrefab;

        // Yeni ghost'u oluştur
        ghostObject = Instantiate(newGhostPrefab, transform);
        
        // Raycast'leri engellemesin diye layer ataması
        SetLayerRecursively(ghostObject, LayerMask.NameToLayer("Ignore Raycast"));

        // Ghost collider'ı -> BoxCollider yoksa ekle, trigger yap
        Collider col = ghostObject.GetComponent<Collider>();
        if (col == null)
            col = ghostObject.AddComponent<BoxCollider>();
        col.isTrigger = true;

        // Opsiyonel: Hala bu ghost'a özel bir texture veya renk vermek istiyorsanız
        if (ghostObjectTexture != null)
        {
            PrototypeVision();
        }
    }

    /// <summary>
    /// Build mod iptal edilirse ghost'u yok edip temizliyoruz.
    /// </summary>
    public void CancelPlacement()
    {
        if (ghostObject != null) 
            Destroy(ghostObject);
        
        ghostObject = null;
        realPrefab = null;
    }

    /// <summary>
    /// (Opsiyonel) Ghost üzerindeki renderer'ların materyalına özel texture/color atama
    /// </summary>
    void PrototypeVision()
    {
        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            // Örnek bir property ID: "Texture2D_4450AB74" demişsiniz, shader'a göre değişebilir
            mat.SetTexture("Texture2D_4450AB74", ghostObjectTexture);
            mat.color = new Color(1f, 1f, 1f, 0.5f);
        }
    }

    /// <summary>
    /// Ghost'un dünyadaki konumunu, mouse'un hit yaptığı yerin grid'e oturtulmuş haline ayarla.
    /// </summary>
    void UpdateGhostPositions()
    {
        if (ghostObject == null) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 point = hit.point;
            Vector3 snappedPosition = new Vector3(
                Mathf.Round(point.x / gridSize) * gridSize,
                0f, // Y'yi sabit değer alıyoruz
                Mathf.Round(point.z / gridSize) * gridSize
            );

            ghostObject.transform.position = snappedPosition;

            // Eğer bu grid pozisyonu doluysa ghost rengi kırmızı olsun
            if (occupiedPositions.Contains(snappedPosition))
                SetGhostColor(Color.red);
            else
                SetGhostColor(new Color(1f, 1f, 1f, 0.1f));
        }

        // Q/E ile ghost'u saat yönünde ters yönde döndürebiliriz
        if (Input.GetKeyDown(KeyCode.Q))
            ghostObject.transform.Rotate(Vector3.up, -90f);
        if (Input.GetKeyDown(KeyCode.E))
            ghostObject.transform.Rotate(Vector3.up, 90f);
    }

    /// <summary>
    /// Hayalet nesnenin etrafında collider var mı, var ise çakışma var mı diye kontrol.
    /// </summary>
    public bool CanPlaceObject()
    {
        if (ghostObject == null) return false;
        BoxCollider bc = ghostObject.GetComponent<BoxCollider>();
        if (bc == null) return false;

        // OverlapBox
        Vector3 centerWS      = bc.transform.TransformPoint(bc.center);
        Vector3 scaledSize    = Vector3.Scale(bc.size, bc.transform.lossyScale);
        Vector3 halfExtentsWS = scaledSize * 0.5f;
        Quaternion rotationWS = bc.transform.rotation;

        Collider[] hits = Physics.OverlapBox(centerWS, halfExtentsWS, rotationWS);
        foreach (Collider c in hits)
        {
            // Ghost'un kendisini, vs. yok sayabiliriz
            if (c.gameObject == ghostObject) continue;
            if (c.name == "womenChecker") continue; // Proje özel bir obje gibi duruyor

            // Herhangi başka bir şeye çarptıysa false
            return false;
        }
        return true;
    }

    /// <summary>
    /// Ghost üzerindeki tüm renderer'lara tek seferde renk ver.
    /// </summary>
    void SetGhostColor(Color color)
    {
        if (ghostObject == null) return;

        Renderer[] renderers = ghostObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material mat = renderer.material;
            mat.color = color;
        }
    }

    /// <summary>
    /// Ghost konumuna realPrefab'ı yerleştir.
    /// </summary>
    public void PlaceObject()
    {
        if (ghostObject == null || realPrefab == null) return;

        Vector3 placementPosition = ghostObject.transform.position;
        if (!occupiedPositions.Contains(placementPosition))
        {
            // Gerçek prefab instantiate
            GameObject placedObject = Instantiate(realPrefab, placementPosition, ghostObject.transform.rotation);

            // Collider trigger değil, normal yap
            Collider placedCol = placedObject.GetComponent<Collider>();
            if (placedCol != null) 
                placedCol.isTrigger = false;

            // Basit DOTween animasyonu (Scale 0'dan 1'e)
            placedObject.transform.localScale = Vector3.zero;
            placedObject.transform
                        .DOScale(Vector3.one, 0.5f)
                        .SetEase(Ease.OutBack);

            // Occupied listeye ekle
            occupiedPositions.Add(placementPosition);
        }
    }

    /// <summary>
    /// Sağ tık ile nesne silme, removeRange içinde ise yok et.
    /// </summary>
    public void RemoveObject()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hit.transform.GetComponent<Breakable>() == null) return;
                if (hit.distance > removeRange) return;
                if (hitObject == ghostObject) return; // Ghost'u silme
                
                Vector3 placedPos = hitObject.transform.position;
                if (occupiedPositions.Contains(placedPos))
                {
                    occupiedPositions.Remove(placedPos);
                    Destroy(hitObject);
                }
            }
        }
    }

    // Debug amaçlı sahnede Ghost'un box'ını çiz
    private void OnDrawGizmos()
    {
        if (ghostObject == null) return;

        BoxCollider bc = ghostObject.GetComponent<BoxCollider>();
        if (bc == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.matrix = bc.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(bc.center, bc.size);
    }

    /// <summary>
    /// Tüm children objelerin layer'ını ayarlar (örneğin "Ignore Raycast").
    /// </summary>
    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }
}
