public class Block {

    private PatternContainer _patterns;
    private int _currentPattern = 0;

    public Pattern GetNextRotationPattern()
    {
        return _patterns[(_currentPattern + 1) % _patterns.Length];
    }

    public Pattern RotatePattern()
    {
        return _patterns[(++_currentPattern) % _patterns.Length];
    }

    public Pattern GetPattern()
    {
        return _patterns[_currentPattern % _patterns.Length];
    }

    public void Init(PatternContainer patterns)
    {
        _patterns = patterns;
    }
}
