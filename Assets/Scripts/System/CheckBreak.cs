public static class CheckBreak
{
    public static System.Collections.IEnumerator CheckBreakSystem()
    {
        while (true)
        {
            yield return new UnityEngine.WaitForFixedUpdate();
        }
    }
}
