using UnityEngine;

namespace Config
{
    public class Environment
    {
        // || Database
        public static string DatabaseFilePath => string.Format("{0}/viamaker.db3", Application.persistentDataPath);
        public static string DatabaseUrl => string.Format("URI=file:{0}", DatabaseFilePath);

        // || API

        public static string BaseAPI => "http://uniescolas.viamaker.com.br/api";
        public static string TokenAPI => "8mspL8yN09CgSQ3sgMfwQkfNm2bO64NW2789Wo0EodONKcuKeUtu1taZjG3Wu5XQUi61uxIZiDqxlxuaoZW9LJ5Hj992DNp6H0pk1wA6h4CZdtZkV6fv5xv8mKcFmkQe";
        public static string FindSchoolUrl => string.Format("{0}/obter/escola", BaseAPI);
        public static string GetStudentsByClassUrl => string.Format("{0}/listar/alunos/turma", BaseAPI);
    }
}