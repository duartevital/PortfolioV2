import type { Photo } from '~/types/photo'

// Placeholder photos using picsum.photos for dev work before the backend is seeded.
// Heights are intentionally mixed to stress-test the masonry layout.
const MOCK: Photo[] = [
  { id: '1',  title: 'Golden Ridge',         category: 'landscape-nature', description: '', shootDate: '2024-08-10', visible: true, order: 0,  thumbnailUrl: 'https://picsum.photos/seed/1/400/560',  displayUrl: 'https://picsum.photos/seed/1/1200/1680', createdAt: '' },
  { id: '2',  title: 'Night Market',         category: 'street-urban',     description: '', shootDate: '2024-11-03', visible: true, order: 1,  thumbnailUrl: 'https://picsum.photos/seed/2/400/266',  displayUrl: 'https://picsum.photos/seed/2/1200/800',  createdAt: '' },
  { id: '3',  title: 'Misty Peaks',          category: 'landscape-nature', description: '', shootDate: '2024-06-22', visible: true, order: 2,  thumbnailUrl: 'https://picsum.photos/seed/3/400/500',  displayUrl: 'https://picsum.photos/seed/3/1200/1500', createdAt: '' },
  { id: '4',  title: 'Under the Bridge',     category: 'street-urban',     description: '', shootDate: '2024-09-14', visible: true, order: 3,  thumbnailUrl: 'https://picsum.photos/seed/4/400/300',  displayUrl: 'https://picsum.photos/seed/4/1200/900',  createdAt: '' },
  { id: '5',  title: 'First Light',          category: 'landscape-nature', description: '', shootDate: '2024-04-05', visible: true, order: 4,  thumbnailUrl: 'https://picsum.photos/seed/5/400/600',  displayUrl: 'https://picsum.photos/seed/5/1200/1800', createdAt: '' },
  { id: '6',  title: 'Corner Store',         category: 'street-urban',     description: '', shootDate: '2024-12-01', visible: true, order: 5,  thumbnailUrl: 'https://picsum.photos/seed/6/400/400',  displayUrl: 'https://picsum.photos/seed/6/1200/1200', createdAt: '' },
  { id: '7',  title: 'Alpine Lake',          category: 'landscape-nature', description: '', shootDate: '2024-07-19', visible: true, order: 6,  thumbnailUrl: 'https://picsum.photos/seed/7/400/267',  displayUrl: 'https://picsum.photos/seed/7/1200/800',  createdAt: '' },
  { id: '8',  title: 'Rain on Glass',        category: 'street-urban',     description: '', shootDate: '2024-10-28', visible: true, order: 7,  thumbnailUrl: 'https://picsum.photos/seed/8/400/533',  displayUrl: 'https://picsum.photos/seed/8/1200/1600', createdAt: '' },
  { id: '9',  title: 'Coastal Fog',          category: 'landscape-nature', description: '', shootDate: '2024-03-11', visible: true, order: 8,  thumbnailUrl: 'https://picsum.photos/seed/9/400/600',  displayUrl: 'https://picsum.photos/seed/9/1200/1800', createdAt: '' },
  { id: '10', title: 'Metro Platform',       category: 'street-urban',     description: '', shootDate: '2024-11-15', visible: true, order: 9,  thumbnailUrl: 'https://picsum.photos/seed/10/400/266', displayUrl: 'https://picsum.photos/seed/10/1200/800', createdAt: '' },
  { id: '11', title: 'Autumn Canopy',        category: 'landscape-nature', description: '', shootDate: '2024-10-02', visible: true, order: 10, thumbnailUrl: 'https://picsum.photos/seed/11/400/500', displayUrl: 'https://picsum.photos/seed/11/1200/1500',createdAt: '' },
  { id: '12', title: 'Empty Lot at Dusk',    category: 'street-urban',     description: '', shootDate: '2024-08-22', visible: true, order: 11, thumbnailUrl: 'https://picsum.photos/seed/12/400/400', displayUrl: 'https://picsum.photos/seed/12/1200/1200',createdAt: '' },
]

export function useMockPhotos(category: Ref<'all' | 'landscape-nature' | 'street-urban'>) {
  const photos = computed(() =>
    category.value === 'all' ? MOCK : MOCK.filter(p => p.category === category.value)
  )
  return { photos }
}
