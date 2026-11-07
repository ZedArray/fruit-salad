using UnityEngine;

// formant-synthesized vowel blips, one per typed vowel
[RequireComponent(typeof(AudioSource))]
public class Babble : MonoBehaviour
{
    [Tooltip("Voice fundamental in Hz. ~120 low, ~220 high.")]
    [SerializeField] private float voiceHz = 120f;
    [SerializeField] private float pitchJitter = 0.08f;
    [SerializeField] private float syllableLength = 0.08f;
    [SerializeField, Range(0, 1)] private float volume = 0.6f;

    private const string Vowels = "aeiou";
    // F1, F2, F3 in Hz (Peterson & Barney averages)
    private static readonly Vector3[] Formants =
    {
        new(730, 1090, 2440), // a
        new(530, 1840, 2480), // e
        new(270, 2290, 3010), // i
        new(570, 840, 2410),  // o
        new(300, 870, 2240),  // u
    };

    private AudioSource source;
    private AudioClip[] clips;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        clips = new AudioClip[Formants.Length];
        for (int i = 0; i < clips.Length; i++) clips[i] = Synth(Vowels[i], Formants[i]);
    }

    public void Say(char c)
    {
        int v = Vowels.IndexOf(char.ToLowerInvariant(c));
        if (v < 0) return;
        source.pitch = 1f + Random.Range(-pitchJitter, pitchJitter);
        source.PlayOneShot(clips[v], volume * SFXManager.VolumeFor(AudioCategory.GameFX));
    }

    private AudioClip Synth(char vowel, Vector3 f)
    {
        int rate = AudioSettings.outputSampleRate;
        int n = (int)(syllableLength * rate);
        float[] data = new float[n];
        Resonator r1 = new(f.x, 90, rate), r2 = new(f.y, 110, rate), r3 = new(f.z, 170, rate);
        float phase = 0, peak = 1e-6f;
        for (int i = 0; i < n; i++)
        {
            phase = (phase + voiceHz / rate) % 1f;
            float glottal = 1f - 2f * phase; // sawtooth, harmonic-rich like vocal folds
            float s = r1.Step(glottal) + 0.5f * r2.Step(glottal) + 0.25f * r3.Step(glottal);
            data[i] = s * Mathf.Sin(Mathf.PI * i / n);
            peak = Mathf.Max(peak, Mathf.Abs(data[i]));
        }
        for (int i = 0; i < n; i++) data[i] /= peak;
        AudioClip clip = AudioClip.Create("babble_" + vowel, n, 1, rate, false);
        clip.SetData(data, 0);
        return clip;
    }

    // two-pole resonator ringing at freq with the given bandwidth
    private struct Resonator
    {
        private readonly float a1, a2, gain;
        private float y1, y2;

        public Resonator(float freq, float bandwidth, int rate)
        {
            float r = Mathf.Exp(-Mathf.PI * bandwidth / rate);
            a1 = 2f * r * Mathf.Cos(2f * Mathf.PI * freq / rate);
            a2 = -r * r;
            gain = 1f - r;
            y1 = y2 = 0;
        }

        public float Step(float x)
        {
            float y = gain * x + a1 * y1 + a2 * y2;
            y2 = y1;
            y1 = y;
            return y;
        }
    }
}
