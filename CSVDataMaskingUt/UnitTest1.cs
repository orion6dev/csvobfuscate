using Cache2Db;

namespace CSVDataMaskingUt;

public class ReplacementTests
{

    [Fact]    public void GetReplacement_SameOriginal_ReturnsSameString()
    {
        var yourClassInstance = new Cache(); // Replace 'YourClassName' with the actual class name containing the GetReplacement method.

        // Arrange
        string original = "testOriginal";

        // Act
        string firstReplacement = yourClassInstance.GetReplacement(original);
        string secondReplacement = yourClassInstance.GetReplacement(original);

        // Assert
        Assert.Equal(firstReplacement, secondReplacement);
    }

    [Fact]
    public void GetReplacement_DifferentOriginal_ReturnsDifferentString()
    {
        var yourClassInstance = new Cache(); // Replace 'YourClassName' with the actual class name containing the GetReplacement method.

        // Arrange
        string original1 = "testOriginal1";
        string original2 = "testOriginal2";

        // Act
        string replacement1 = yourClassInstance.GetReplacement(original1);
        string replacement2 = yourClassInstance.GetReplacement(original2);

        // Assert
        Assert.NotEqual(replacement1, replacement2);
    }
}
