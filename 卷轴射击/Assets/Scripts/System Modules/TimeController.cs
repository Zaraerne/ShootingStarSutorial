using System.Collections;
using UnityEngine;

/// <summary>
/// 时间控制
/// </summary>
public class TimeController : Singleton<TimeController>
{
    [Header("时间缩放尺度")]
    [SerializeField, Range(0f, 1.0f)] float bulletTimeScale = 0.1f;
    
    float t;
    /// <summary>
    /// 固定的间隔时间
    /// </summary>
    float defaultFixedDeltaTime;
    /// <summary>
    /// 记录时间缩放前的缩放尺度
    /// </summary>
    float timeScaleBeforePause;


    protected override void Awake()
    {
        base.Awake();
        // 记录当前的固定更新间隔
        defaultFixedDeltaTime = Time.fixedDeltaTime;
    }
    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        // 记录当前的时间缩放尺度
        timeScaleBeforePause = Time.timeScale;
        // 所有时间暂停
        // 注意：如果这里修改为0，因为Animator的默认更新是按照timeScale进行更新
        // 所以所有的非 Unscale Time Animator全部会被冻结，无法播放动画，需要自己进行设置 Animator 的updateMode为 Unscale Time模式
        Time.timeScale = 0f;
    }
    /// <summary>
    /// 解除缩放
    /// </summary>
    public void UnPause()
    {
        // 解除时间暂停，回到之前的时间缩放值
        Time.timeScale = timeScaleBeforePause;
    }

    /// <summary>
    /// 子弹时间 Time.timeScale -- 1
    /// </summary>
    /// <param name="duration">缩放时间</param>
    public void BulletTime(float duration)
    {
        Time.timeScale = bulletTimeScale;
        StartCoroutine(SlowOutCoroutine(duration));
    }
    /// <summary>
    /// 子弹时间 Time.timeScale -inDuration- bulletTimeScale -outDuration- 1
    /// </summary>
    /// <param name="inDuration">进入时间</param>
    /// <param name="outDuration">退出时间</param>
    public void BulletTime(float inDuration, float outDuration)
    {
        Time.timeScale = bulletTimeScale;
        StartCoroutine(SlowInAndOutCoroutine(inDuration, outDuration));
    }

    /// <summary>
    /// 子弹时间 Time.timeScale -inDuration- bulletTimeScale -keepDuration- bulletTimeScale -outDuration- 1
    /// </summary>
    /// <param name="inDuration">进入时间</param>
    /// <param name="keepDuration">保持时间</param>
    /// <param name="outDuration">退出时间</param>
    public void BulletTime(float inDuration, float keepDuration, float outDuration)
    {
        StartCoroutine(SlowInKeepAndOutDuration(inDuration, keepDuration, outDuration));
    }
    /// <summary>
    /// 进入
    /// </summary>
    /// <param name="duration"></param>
    public void SlowIn(float duration)
    {
        StartCoroutine(SlowInCoroutine(duration));
    }
    /// <summary>
    /// 退出
    /// </summary>
    /// <param name="duration"></param>
    public void SlowOut(float duration)
    {
        StartCoroutine(SlowOutCoroutine(duration));
    }

    IEnumerator SlowInKeepAndOutDuration(float inDuration,float keepDuration,float outDuration)
    {
        yield return SlowInCoroutine(inDuration);
        yield return new WaitForSecondsRealtime(keepDuration);
        StartCoroutine(SlowOutCoroutine(outDuration));
    }

    IEnumerator SlowInAndOutCoroutine(float inDuration,float outDuration)
    {
        yield return StartCoroutine(SlowInCoroutine(inDuration));
        StartCoroutine(SlowOutCoroutine(outDuration));
    }
    /// <summary>
    /// 淡入携程
    /// </summary>
    /// <param name="duration">淡入时间</param>
    /// <returns></returns>
    IEnumerator SlowInCoroutine(float duration)
    {
        t = 0;
        while (t < 1)
        {
            if (GameManager.GameState != GameState.Paused)
            {
                // 记录正式记录的时间
                t += Time.unscaledDeltaTime;
                // 对时间缩放进行插值
                Time.timeScale = Mathf.Lerp(1f, bulletTimeScale, t);
                // 修改固定的更新时间
                Time.fixedDeltaTime = defaultFixedDeltaTime * bulletTimeScale;
            }
            yield return null;
        }
    }

    /// <summary>
    /// 淡出携程
    /// </summary>
    /// <param name="duration">淡出时间</param>
    /// <returns></returns>
    IEnumerator SlowOutCoroutine(float duration)
    {
        t = 0;
        while(t < 1)
        {
            if(GameManager.GameState != GameState.Paused)
            {
                t += Time.unscaledDeltaTime;
                Time.timeScale = Mathf.Lerp(bulletTimeScale, 1f, t);
                Time.fixedDeltaTime = defaultFixedDeltaTime * bulletTimeScale;
            }
            yield return null;
        }
    }
}
