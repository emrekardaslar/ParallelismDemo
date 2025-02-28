using System.Collections.Generic;

public class ScriptCollector
{
    private readonly List<string> _scripts = new();

    public void AppendScript(string script)
    {
        if (!_scripts.Contains(script))
        {
            _scripts.Add(script);
        }
    }

    public IEnumerable<string> GetScripts()
    {
        return _scripts;
    }

    public void Clear()
    {
        _scripts.Clear();
    }
}
