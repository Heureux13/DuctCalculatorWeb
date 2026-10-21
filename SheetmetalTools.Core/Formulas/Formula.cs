namespace SheetmetalTools.Formulas;

public class Formula
{
    /// <summary>
    /// Makes sure the number is greater than zero
    /// </summary>
    /// <param name="number">the number you want to validate.</param>
    /// <param name="parameterName"> name of the parameter you are assiging the number to.</param>
    /// <returns>Number greater than zero.</returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    #region Validate Number
    public double ValidateNumber(double number, string parameterName)
    {
        if (!double.IsFinite(number) || number <= 0)
        {
            throw new ArgumentOutOfRangeException(parameterName,
                "Number must be finite and greater than zero.");
        }
        return number;
    }
    #endregion

    public string Clean(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    /// <summary>
    /// Calculates Hydraulic Diameter.
    /// </summary>
    /// <param name="AreaIn2">Area in squre inches.</param>
    /// <param name="wettedPerimeter">Perimeter of duct incontact with air in inches]</param>
    /// <returns>Hydraulid diamter in inches.</returns>
    #region Hydraulic Diameter
    public double HydraulicDiameter(double AreaIn2, double wettedPerimeter)
    {
        double area = ValidateNumber(AreaIn2, nameof(AreaIn2));
        double perimeter = ValidateNumber(wettedPerimeter, nameof(wettedPerimeter));

        return 4 * area / perimeter;
    }
    #endregion

    /// <summary>
    /// Calculates Kinematic Viscosity
    /// </summary>
    /// <param name="dynamicViscosityPaS"></param>
    /// <param name="airDensityKgperM3"></param>
    /// <returns>Kinematic Viscosity in square feet per second.</returns>
    #region Kinematic Viscosity
    public double KinematicViscosityFt2PerS(double dynamicViscosityPaS, double airDensityKgperM3)
    {
        double viscosity = dynamicViscosityPaS;
        double density = ValidateNumber(airDensityKgperM3, nameof(airDensityKgperM3));

        return (viscosity / density) * 10.7639;
    }
    #endregion

    #region Darcy Friction Factor
    public double DarcyFrictionFactor(double reynoldsNumber, double roughness, double hydraulicDiameter)
    {
        var Re = ValidateNumber(reynoldsNumber, nameof(reynoldsNumber));
        var E = ValidateNumber(roughness, nameof(roughness));
        var D = ValidateNumber(hydraulicDiameter, nameof(hydraulicDiameter));

        return 0.25 / Math.Pow(
            Math.Log10(
            (E / (3.7 * D))
            + (5.74 / Math.Pow(Re, 0.9))
            ), 2);

    }
    #endregion

    /// <summary>
    /// Calculates Reynolds number from velocity, hydraulic diamter, and kinematic visocosity.
    /// </summary>
    /// <param name="velocityFpm">Velocity in eet per minute.</param>
    /// <param name="hydraulicDiameterIn">Hydraulic diameter in inches.</param>
    /// <param name="kinematicViscosityFt2PerS">square feet per second.</param>
    /// <returns>Reynodls number.</returns>
    #region Reynolds Number
    public double ReynoldsNumber(double velocityFpm, double hydraulicDiameterIn, double kinematicViscosityFt2PerS)
    {
        double velocity = ValidateNumber(velocityFpm, nameof(velocityFpm)) / 60.0;
        double diameter = ValidateNumber(hydraulicDiameterIn, nameof(hydraulicDiameterIn)) / 12.0;
        double viscosity = ValidateNumber(kinematicViscosityFt2PerS, nameof(kinematicViscosityFt2PerS));

        return velocity * diameter / viscosity;
    }
    #endregion

    /// <summary>
    /// Darcy Weisback Number
    /// </summary>
    /// <param name="darcyFrictionFactor"></param>
    /// <param name="hydraulicDiameterIn"></param>
    /// <param name="airDensity"></param>
    /// <param name="airVelocity"></param>
    /// <returns></returns>
    #region Darcy Weisbach Number
    public double DarcyWeisbach(
                double darcyFrictionFactor,
                double hydraulicDiameterIn,
                double airDensity,
                double airVelocity)
    {
        return darcyFrictionFactor 
            * (100 / hydraulicDiameterIn)
            * (airDensity * Math.Pow(airVelocity, 2) / 2);
    }
    #endregion

    #region ASHRAE
    public double ASHRAE(double widthIn, double heightIn)
    {
        var width = ValidateNumber(widthIn, nameof(widthIn));
        var height = ValidateNumber(heightIn, nameof(heightIn));

        return 1.30
            * Math.Pow((width * height), 0.625)
            / Math.Pow(width + height, 0.25);
    }
    #endregion

    /// <summary>
    /// Uses Sutherland's Equation to find dynamic viscosity
    /// </summary>
    /// <param name="temperatureK">Tmeperature in kelven.</param>
    /// <returns>Dynamic viscosity in Pa S</returns>
    #region Sutherland's Equation
    public double DynamicViscosity(double temperatureK)
    {
        double U0 = 0.00001716; // Pa*s
        double T0 = 273.15; // K
        double S = 111; // K
        var T = ValidateNumber(temperatureK, nameof(temperatureK));

        return U0
            * Math.Pow(T / T0, 3.0 / 2.0)
            * ((T0 + S) / (T + S));
    }
    #endregion

    /// <summary>
    /// The amount of pressure air loses as it travels through the duct because of friction with the ductwalls
    /// </summary>
    /// <param name="frictionLoss">Friction loss in Lb/Ft²</param>
    /// <returns>Water gauge in 100 feet</returns>
    #region Friction Loss
    public double FrictionLoss(double frictionFactor, double hydraulicDiameter, double airDensity, double velocity)
    {
        var f = frictionFactor;
        var dFt = hydraulicDiameter / 12; // inches divided by 12 inches in a foot = feet
        var rho = airDensity;
        var vFps = velocity / 60; // feet per minute divided by 60 seconds in a minute = feet per second

        double lbfPerFt2 = f * (100.0 / dFt) * (rho * vFps * vFps) / (2 * 32.174);

        return lbfPerFt2 / 5.202;
    }
    #endregion

    /// <summary>
    /// Allowable Leakage
    /// </summary>
    /// <param name="leakClass"></param>
    /// <param name="surfaceArea"></param>
    /// <param name="waterGauge"></param>
    /// <returns></returns>
    #region Allowable Leakage
    public double AllowableLeakage(double leakClass, double surfaceArea, double waterGauge)
    {
        return leakClass * (surfaceArea / 100) * Math.Pow(waterGauge, 0.65);
    }
    #endregion
}
