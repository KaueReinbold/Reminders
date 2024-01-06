import styles from './page.module.css'
import RemindersList from './reminder/list/page';

export default function Home() {
  return (
    <main className={styles.main}>
      <RemindersList />
    </main>
  );
}