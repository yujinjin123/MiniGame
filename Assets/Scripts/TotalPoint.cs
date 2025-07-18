using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TotalPoint : Singleton<TotalPoint>
{
    public Text pointText;
    public Text pointIncrease; // 显示加分文本的UI
    public CanvasGroup pointIncreaseCanvasGroup; // 控制透明度的组件
    public static int currentPoint = 0;
    public Text gameOver;//游戏结束文本
    public float fadeDuration = 1.5f; // 淡出持续时间（秒）

    protected override void Awake()
    {
        base.Awake();
        currentPoint = 0;
        pointIncreaseCanvasGroup = pointIncrease.GetComponent<CanvasGroup>();
        pointIncreaseCanvasGroup.alpha = 0; // 初始化时隐藏
        gameOver = GetComponentInChildren<Text>();
        gameOver.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        pointText.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(true);
        gameOver.text = $"游戏结束\n总得分:{currentPoint}";
    }

    public void GetPoint(int num)
    {
        currentPoint += num;
        pointText.text = currentPoint.ToString();
        // 1. 更新加分文本内容
        pointIncrease.text = $"+{num}";

        // 2. 重置透明度并启用显示
        pointIncreaseCanvasGroup.alpha = 1;
        pointIncrease.gameObject.SetActive(true);

        // 3. 启动淡出协程
        StartCoroutine(FadeTextCoroutine());
    }

    private IEnumerator FadeTextCoroutine()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            // 随时间减少透明度（线性渐变）
            pointIncreaseCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 4. 淡出完成后隐藏并重置状态
        pointIncreaseCanvasGroup.alpha = 0;
        pointIncrease.gameObject.SetActive(false);
    }
}


// 泛型单例基类（无需挂载到物体）
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock) // 线程安全锁[3,6](@ref)
            {
                if (_instance == null)
                {
                    // 查找场景中现有实例
                    _instance = FindObjectOfType<T>();
                    if (_instance == null)
                    {
                        // 创建新GameObject挂载单例
                        GameObject obj = new GameObject(typeof(T).Name);
                        _instance = obj.AddComponent<T>();
                        DontDestroyOnLoad(obj); // 跨场景不销毁[2,5](@ref)
                    }
                }
                return _instance;
            }
        }
    }

    protected virtual void Awake()
    {
        // 防止重复实例
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject); // 销毁多余实例[1](@ref)
        }
        else
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject); // 持久化
        }
    }
}