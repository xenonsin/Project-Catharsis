namespace Catharsis.GlobalVariablesManager
{
    public interface IGlobalVariableManager
    {
        void loadGlobalVariables(string globalVariablesXml);

        void saveGlobalVariables();


        float GetFloat(int id);

        void SetFloat(int id, float value);
        bool GetBool(int id);

        void SetBool(int id, bool value);

        string GetString(int id);

        void SetString(int id, string value);
    }
}