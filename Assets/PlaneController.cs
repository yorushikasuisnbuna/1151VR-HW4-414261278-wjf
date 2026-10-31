using UnityEngine;
using UnityEngine.SceneManagement;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody))]
public class PlaneController : MonoBehaviour
{
    [Header("Flight")]
    public float forwardSpeed = 12f;
    public float moveSpeed = 8f;
    public float xLimit = 5f;
    public float yMin = 1f;
    public float yMax = 6f;
    public float endZ = 160f;

    [Header("Sounds")]
    public AudioClip flySound;
    public AudioClip crashSound;
    public AudioClip passSound;

    AudioSource loopSource;
    AudioSource sfxSource;
    Vector3 target;
    bool finished = false;
    int passed = 0;
    int hits = 0;

    void Awake()
    {
        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.clip = flySound;
        loopSource.loop = true;
        loopSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;
        sfxSource.loop = false;
    }

    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        target = transform.position;
        if (flySound != null) loopSource.Play();
    }

    void Update()
    {
        if (finished)
        {
            if (PressedThisFrame())
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        Vector2 p;
        if (GetPointer(out p))
        {
            float nx = Mathf.Clamp01(p.x / Screen.width);
            float ny = Mathf.Clamp01(p.y / Screen.height);
            target.x = Mathf.Lerp(-xLimit, xLimit, nx);
            target.y = Mathf.Lerp(yMin, yMax, ny);
        }

        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards(pos.x, target.x, moveSpeed * Time.deltaTime);
        pos.y = Mathf.MoveTowards(pos.y, target.y, moveSpeed * Time.deltaTime);
        pos.z += forwardSpeed * Time.deltaTime;
        transform.position = pos;

        if (pos.z >= endZ)
        {
            finished = true;
            loopSource.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Obstacle ob = other.GetComponentInParent<Obstacle>();
        if (ob == null || ob.hit || finished) return;
        ob.hit = true;
        hits++;
        PlaySfx(crashSound);
    }

    public void OnPassedObstacle()
    {
        passed++;
        PlaySfx(passSound);
    }

    void PlaySfx(AudioClip clip)
    {
        if (clip != null) sfxSource.PlayOneShot(clip);
    }

    bool GetPointer(out Vector2 pos)
    {
#if ENABLE_INPUT_SYSTEM
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            pos = Touchscreen.current.primaryTouch.position.ReadValue();
            return true;
        }
        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            pos = Mouse.current.position.ReadValue();
            return true;
        }
        pos = Vector2.zero;
        return false;
#else
        pos = Input.mousePosition;
        return Input.GetMouseButton(0);
#endif
    }

    bool PressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        return (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            || (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame);
#else
        return Input.GetMouseButtonDown(0);
#endif
    }

       void OnGUI()
    {
        Rect safe = Screen.safeArea;
        float margin = Screen.height * 0.05f;
        float left = safe.x + margin;
        float top = (Screen.height - safe.yMax) + margin;
        float width = safe.width - margin * 2f;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = Mathf.RoundToInt(Screen.height / 20f);
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(left, top, width, Screen.height / 8f),
                  "Passed: " + passed + "   Hits: " + hits, style);
        if (finished)
            GUI.Label(new Rect(left, Screen.height / 2f, width, Screen.height / 8f),
                      "Finish! Tap to restart", style);
    }
}
