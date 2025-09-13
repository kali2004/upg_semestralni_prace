Tento projekt tvoøí kostru semestrální práce KIV/UPG 2025/2026 a je vyuitelnı zejména pro studenty, kteøí se rozhodnou semestrální práci vypracovat v C# s vyuitím Avalonia UI a SkiaSharp. 
Projekt je k dispozici ke staení na https://gitlab.kiv.zcu.cz/UPG/dronewar.git. 

*UPOZORNÌNÍ:* Pøed odevzdáním (na https://portal.zcu.cz/portal/studium/moje-vyuka/odevzdavani-praci.html) odstraòte všechny soubory, které nejsou pro odevzdávanou práci relevantní, tj. napø. pokud pro dokumentaci jste šablonu dokumentace.dot nevyuili, ale vyuili jste pouze dokumentace.docx, ponechte soubor .docx; analogicky, pokud Vaše práce má bıt spuštìna pod Windows, ponechte skripty Run.cmd a Build.cmd, pokud pod Linux/Mac, ponechte skripty Run.sh a Build.sh, oba skripty ponechte, pokud práci lze spouštìt jak na Windows tak na Linux/Mac.

*UPOZORNÌNÍ:* Kostra se v prùbìhu semestru mùe zmìnit. Studentùm je proto doporuèeno, aby pro svou práci vyuili verzovací systém Git, a to tak, e mají dva vzdálené repozitáøe: vlastní origin pro FETCH/PULL/PUSH, zaloenı napø. na github / gitlab, a upstream pro FETCH/PULL vedoucí na pùvodní zdroj a na vızvu pøednášejícího/cvièícího provedli FETCH/PULL z upstream.



Návod pro pouití s GIT pro úplné zaèáteèníky
---------------------------------------------
1. Pokud ji máte zøízen nìkde GIT úèet umoòující vám zakládat soukromé (private) repozitáøe, jdìte na krok 3
2. Zalote si úèet na https://gitlab.com/ nebo https://bitbucket.org nebo https://github.com. 
3. Zalote novı SOUKROMİ (private) repozitáø a nìjak vhodnì si ho pojmenujte. Rady, a zaloíte .gitignore nebo soubor readme, ignorujte - vy ji máte svùj existující projekt.
4. Získejte HTTPS adresu k vašemu repozitáøi (bıvá zøetelnì uvedena).
5. Pokud jste tento projekt získali doporuèenım klonováním z Git (máte zde skrytı podadresáø .git), jdìte na krok 8.
6. Prostøednictvím TortoiseGit (vyvolá se z kontextového menu v prùzkumníkovi) nebo Git Extensions (èi jinıch) zalote lokální repozitáø v tomto adresáøi (Git Create repository here ...). Od této chvíle mùete provádìt "commit" a uchovávat lokálnì zmìny.
7. Vyvolejte Git Commit a všechny soubory "commitujte" do lokálního repozitáøe (poèáteèní/první commit).
8. Vyvolejte Push a v nastavení "Remote" pøidejte novı vzdálenı repozitáø s názvem "origin" a jako URL volte tu, kterou jste získali v kroku 4 (tj. HTTPS adresu k vašemu repozitáøi). Dokonèete Push. TortoiseGit si bìhem toho vyádá vaše pøihlašovací údaje a všechny vaše zmìny (lokálnì vedené v podadresáøi .git) zkopíruje do vzdáleného repozitáøe.
8. Zkontrolujte, e data jsou skuteènì uloena.

Poznámka: vyuijte soubor .gitignore pro specifikaci automaticky generovanıch souborù (.class, javadoc dokumentace, apod.), aby se tyto soubory neukládaly do Git repozitáøù.
