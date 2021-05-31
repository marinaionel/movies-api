/* delete invalid people references in directors */
DELETE
FROM moviesfile.directors
where person_id in (SELECT person_id
                    from moviesfile.directors
                    where person_id not in (select id from moviesfile.people));

/* delete invalid people references in stars*/
DELETE
FROM moviesfile.stars
where person_id in (SELECT person_id
                    from moviesfile.stars
                    where person_id not in (select id from moviesfile.people));

/* find duplicates in stars */
SELECT movie_id, person_id, COUNT(*)
FROM moviesfile.stars
GROUP BY movie_id, person_id
HAVING COUNT(*) > 1;

/* remove duplicates in stars */
WITH cte AS (
    SELECT movie_id,
           person_id,
           ROW_NUMBER() OVER (
               PARTITION BY
               movie_id,
               person_id
               ORDER BY
                   movie_id,
                   person_id
               ) row_num
    FROM moviesfile.stars
)
DELETE
FROM cte
WHERE row_num > 1;