/* No dependencies, awww yeahhh. */

namespace Testsender
{
    /// <summary>
    /// Could be implemented by the API if necessary.
    /// </summary>
    internal interface ILightsControllerService
    {
        /// <summary>
        /// Updates the light state.
        /// Might fail, if so these functions will return false.
        /// </summary>
        bool UpdateLightState(int light, bool state, byte brightness, int hue, int saturation);
        bool UpdateLightState(int light, bool state, byte brightness);

        /// <summary>
        /// Not really, more like 
        /// GetInternalRepresentationOfStateThatMightOrMightNotBeTrue(int light), 
        /// but that would have been too long-winded. Same goes for GetBrightness(int light).
        /// </summary>
        string GetState(int light);
        byte GetBrightness(int light);
    } // End of ILightsControllerService
} // End of Testsender
