
public class Queries
{
    public class Escola
    {
        public static string CreateTable
        {
            get => " CREATE TABLE IF NOT EXISTS escola ( id INTEGER PRIMARY KEY AUTOINCREMENT,  nome TEXT NOT NULL ) ";
        }
        public static string Insert => " INSERT INTO escola (nome) VALUES (@nome) ";
        public static string Update => " UPDATE escola SET nome = @nome WHERE id = @id ";
        public static string DeleteById => " DELETE FROM escola WHERE id = @id ";
        public static string FindAll => " SELECT id, nome FROM escola ORDER BY nome ASC ";
        public static string FindById => " SELECT id, nome FROM escola WHERE id = @id ";
    }

    public class Turma
    {
        public static string CreateTable
        {
            get => @" CREATE TABLE IF NOT EXISTS turma 
                      ( id INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT NOT NULL, escola_id INTEGER NOT NULL, 
                        FOREIGN KEY (escola_id) REFERENCES escola (id) ) ";
        }
        public static string Insert => " INSERT INTO turma (nome, escola_id) VALUES (@nome, @escola_id) ";
        public static string InsertMultiples => " INSERT INTO turma (nome, escola_id) VALUES ('{0}', {1}) ";
        public static string Update => " UPDATE turma SET nome = @nome WHERE id = @id AND escola_id = @escola_id ";
        public static string DeleteById => " DELETE FROM turma WHERE id = @id ";
        public static string DeleteAllByEscola => " DELETE FROM turma WHERE escola_id = @escola_id ";
        public static string FindByEscola => @" SELECT turma.id, turma.nome FROM turma AS turma
                                                INNER JOIN escola AS escola ON (turma.escola_id = escola.id)
                                                WHERE escola.id = @id
                                                ORDER BY turma.id ASC ";
        public static string FindById => " SELECT id, nome FROM turma WHERE id = @id ";
    }

    public class Aluno
    {
        public static string CreateTable
        {
            get => @" CREATE TABLE IF NOT EXISTS aluno
                      ( id INTEGER PRIMARY KEY AUTOINCREMENT, nome TEXT NOT NULL, turma_id INTEGER NOT NULL,
                      FOREIGN KEY (turma_id) REFERENCES turma (id) ) ";
        }
        public static string Insert => " INSERT INTO aluno (nome, turma_id) VALUES (@nome, @turma_id) ";
        public static string InsertMultiples => " INSERT INTO aluno (nome, turma_id) VALUES ('{0}', {1}); ";
        public static string Update => " UPDATE aluno SET nome = @nome WHERE id = @id AND turma_id = @turma_id ";
        public static string Delete => " DELETE FROM aluno WHERE id = @id ";
        public static string DeleteAllByTurma => " DELETE FROM aluno WHERE turma_id = @turma_id ";
        public static string FindByTurma => @" SELECT aluno.id, aluno.nome FROM aluno AS aluno 
                                               INNER JOIN turma AS turma ON (aluno.turma_id = turma.id)
                                               WHERE turma.id = @turma_id
                                               ORDER BY aluno.nome ASC ";
        public static string FindById => " SELECT id, nome FROM aluno WHERE id = @id ";
    }
}